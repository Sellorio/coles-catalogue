using ColesCatalogue.Logic.Data;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http;
using Fizzler.Systems.HtmlAgilityPack;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Net;

namespace ColesCatalogue.Logic.Services
{
    internal class SpecialsService : ISpecialsService
    {
        const string SpecialsItemsQuery = ".coles-targeting-ProductTileProductTileWrapper";
        const string PaginationPageCountQuery = ".coles-targeting-PaginationPaginationRoot li:nth-last-child(2) a";
        const string SpecialDescripionQuery = ".product__badge";
        const string ProductTitleQuery = ".product__title";
        const string ProductLinkQuery = ".product__link";
        const string ProductImageQuery = "img[data-testid=product-image]";
        const string CurrentPriceQuery = ".price span:nth-child(1)";
        const string SavingsAmountQuery = ".price span:nth-child(2)";
        const string PrizePerAmountDescriptionQuery = ".price__calculation_method";

        const string SpecialsDataKey = "specials";
        const string SpecialsCachedAtDataKey = "specialsSavedAt";

        private readonly HttpClient _httpClient;
        private readonly IStorageService _storageService;

        public SpecialsService(HttpClient httpClient, IStorageService storageService)
        {
            _httpClient = httpClient;
            _storageService = storageService;
        }

        public async Task<IList<SpecialsItem>> GetSpecialsAsync(ProgressTracker progressTracker = null)
        {
            if (progressTracker != null)
            {
                progressTracker.Max = 1;
                progressTracker.Current = 1;
            }

            var cachedAt = await _storageService.ReadAsync<DateTime?>(SpecialsCachedAtDataKey);

            if (cachedAt != null && DateTime.Today - cachedAt.Value.Date < TimeSpan.FromDays(7))
            {
                cachedAt = cachedAt.Value.Date.AddDays(1);

                while (cachedAt.Value <= DateTime.Today)
                {
                    if (cachedAt.Value.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        cachedAt = null;
                        break;
                    }

                    cachedAt = cachedAt.Value.AddDays(1);
                }

                if (cachedAt != null)
                {
                    return await _storageService.ReadAsync<List<SpecialsItem>>(SpecialsDataKey);
                }
            }

            var html = await _httpClient.GetStringAsync("https://www.coles.com.au/on-special?page=1&sortBy=salesDescending");
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var pageCount = int.Parse(document.DocumentNode.QuerySelector(PaginationPageCountQuery).InnerText);

            if (progressTracker != null)
            {
                progressTracker.Max = pageCount;
            }

            var result = new List<SpecialsItem>();

            result.AddRange(ParseSpecialsPage(document));

            for (int i = 1; i <= pageCount; i++)
            {
                if (progressTracker != null)
                {
                    progressTracker.Current = i;
                }

                // delay page downloads to not overload coles website or trigger any spam detection
                await Task.Delay(3000);

                html = await _httpClient.GetStringAsync($"https://www.coles.com.au/on-special?page={i}&sortBy=salesDescending");
                document = new HtmlDocument();
                document.LoadHtml(html);

                result.AddRange(ParseSpecialsPage(document).Where(x => result.All(y => y.ItemId != x.ItemId)));
            }

            await _storageService.WriteDataAsync(SpecialsCachedAtDataKey, DateTime.Now);
            await _storageService.WriteDataAsync(SpecialsDataKey, result);

            return result;
        }

        private static IList<SpecialsItem> ParseSpecialsPage(HtmlDocument document)
        {
            var result = new List<SpecialsItem>();
            var products = document.DocumentNode.QuerySelectorAll(SpecialsItemsQuery);

            foreach (var product in products)
            {
                int itemId = int.Parse(Regex.Match(product.QuerySelector(ProductLinkQuery).GetAttributeValue<string>("href", null), "[0-9]+$").Value);
                string itemName = WebUtility.HtmlDecode(product.QuerySelector(ProductTitleQuery).InnerText);
                string itemImageUrl = product.QuerySelectorAll(ProductImageQuery).Last().GetAttributeValue<string>("src", null);
                itemImageUrl = WebUtility.UrlDecode(itemImageUrl.Substring(17, itemImageUrl.IndexOf('&') - 17));

                string specialDescription = product.QuerySelector(SpecialDescripionQuery)?.InnerText;

                // invalid data that is missing core fields
                if (specialDescription == null)
                {
                    continue;
                }

                decimal currentPrice = decimal.Parse(product.QuerySelector(CurrentPriceQuery).InnerText.Substring(1));

                var savingsAmountNode = product.QuerySelector(SavingsAmountQuery);

                decimal savingsAmount = savingsAmountNode == null ? 0.0M : decimal.Parse(savingsAmountNode.InnerText.Substring(6));

                decimal normalPrice = currentPrice + savingsAmount;

                decimal savingsPercent = (1 - currentPrice / normalPrice) * 100;

                string currentPricePerAmountDescription = product.QuerySelector(PrizePerAmountDescriptionQuery)?.GetDirectInnerText();
                string normalPricePerAmountDescription =
                    currentPricePerAmountDescription == null
                        ? null
                        : Regex.Replace(
                            currentPricePerAmountDescription,
                            @"^(\$)([0-9\.]+)( per [0-9]+[a-zA-Z]+)$",
                            x =>
                            {
                                return
                                    x.Groups[1].Value +
                                    (decimal.Parse(x.Groups[2].Value) * normalPrice / currentPrice).ToString("N2") +
                                    x.Groups[3].Value;
                            });

                result.Add(new SpecialsItem
                {
                    ItemId = itemId,
                    ItemName = itemName,
                    ItemImageUrl = itemImageUrl,
                    SpecialDescription = specialDescription,
                    NormalPrice = normalPrice,
                    CurrentPrice = currentPrice,
                    SavingsPercent = savingsPercent,
                    NormalPricePerAmountDescription = normalPricePerAmountDescription,
                    CurrentPricePerAmountDescription = currentPricePerAmountDescription
                });
            }

            return result;
        }
    }
}