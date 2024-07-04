using FairFlexxApps.Capture.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace FairFlexxApps.Capture.Models
{
    public class Country
    {
        public string GermanyName { get; set; }
        public string EnglishName { get; set; }
    }
    public class CountryService
    {
        public static List<Country> CountryList()
        {
            var type = new List<Country>()
            {
                new Country() {GermanyName = "Vereinigte Staaten", EnglishName="United States"},
                new Country() {GermanyName = "Kanada", EnglishName="Canada"},
                new Country() {GermanyName = "Vereinigtes Königreich", EnglishName="United Kingdom"},
                new Country() {GermanyName = "Frankreich", EnglishName="France"},
                new Country() {GermanyName = "Deutschland", EnglishName="Germany"},
                new Country() {GermanyName = "Italien", EnglishName="Italy"},
                new Country() {GermanyName = "Spanien", EnglishName="Spain"},
                new Country() {GermanyName = "Niederlande", EnglishName="Netherlands"},
                new Country() {GermanyName = "Schweiz", EnglishName="Switzerland"},
                new Country() {GermanyName = "Österreich", EnglishName="Austria"},
                new Country() {GermanyName = "Belgien", EnglishName="Belgium"},
                new Country() {GermanyName = "Schweden", EnglishName="Sweden"},
                new Country() {GermanyName = "Norwegen", EnglishName="Norway"},
                new Country() {GermanyName = "Dänemark", EnglishName="Denmark"},
                new Country() {GermanyName = "Finnland", EnglishName="Finland"},
                new Country() {GermanyName = "Irland", EnglishName="Ireland"},
                new Country() {GermanyName = "Australien", EnglishName="Australia"},
                new Country() {GermanyName = "Neuseeland", EnglishName="New Zealand"},
                new Country() {GermanyName = "Südkorea", EnglishName="South Korea"},
                new Country() {GermanyName = "Indien", EnglishName="India"},
                new Country() {GermanyName = "Russland", EnglishName="Russia"},
                new Country() {GermanyName = "Brasilien", EnglishName="Brazil"},
                new Country() {GermanyName = "Mexiko", EnglishName="Mexico"},
                new Country() {GermanyName = "Argentinien", EnglishName="Argentina"},
                new Country() {GermanyName = "Kolumbien", EnglishName="Colombia"},
                new Country() {GermanyName = "Südafrika", EnglishName="South Africa"},
                new Country() {GermanyName = "Ägypten", EnglishName="Egypt"},
                new Country() {GermanyName = "Marokko", EnglishName="Morocco"},
            };
            return type;
        }
    }
}
