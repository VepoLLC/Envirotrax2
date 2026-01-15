using Envirotrax.App.Server.Data.Models.States;

namespace Envirotrax.App.Server.Data.SeedData
{
    public static class StateSeedData
    {
        public static IReadOnlyList<State> States => new List<State>
        {
            new() { Id = 1,  Name = "International (Outside the USA)", Code = "INT" },
            new() { Id = 2,  Name = "Alabama", Code = "AL" },
            new() { Id = 3,  Name = "Alaska", Code = "AK" },
            new() { Id = 4,  Name = "Arizona", Code = "AZ" },
            new() { Id = 5,  Name = "Arkansas", Code = "AR" },
            new() { Id = 6,  Name = "California", Code = "CA" },
            new() { Id = 7,  Name = "Colorado", Code = "CO" },
            new() { Id = 8,  Name = "Connecticut", Code = "CT" },
            new() { Id = 9,  Name = "Delaware", Code = "DE" },
            new() { Id = 10, Name = "District of Columbia", Code = "DC" },
            new() { Id = 11, Name = "Florida", Code = "FL" },
            new() { Id = 12, Name = "Georgia", Code = "GA" },
            new() { Id = 13, Name = "Hawaii", Code = "HI" },
            new() { Id = 14, Name = "Idaho", Code = "ID" },
            new() { Id = 15, Name = "Illinois", Code = "IL" },
            new() { Id = 16, Name = "Indiana", Code = "IN" },
            new() { Id = 17, Name = "Iowa", Code = "IA" },
            new() { Id = 18, Name = "Kansas", Code = "KS" },
            new() { Id = 19, Name = "Kentucky", Code = "KY" },
            new() { Id = 20, Name = "Louisiana", Code = "LA" },
            new() { Id = 21, Name = "Maine", Code = "ME" },
            new() { Id = 22, Name = "Maryland", Code = "MD" },
            new() { Id = 23, Name = "Massachusetts", Code = "MA" },
            new() { Id = 24, Name = "Michigan", Code = "MI" },
            new() { Id = 25, Name = "Minnesota", Code = "MN" },
            new() { Id = 26, Name = "Mississippi", Code = "MS" },
            new() { Id = 27, Name = "Missouri", Code = "MO" },
            new() { Id = 28, Name = "Montana", Code = "MT" },
            new() { Id = 29, Name = "Nebraska", Code = "NE" },
            new() { Id = 30, Name = "Nevada", Code = "NV" },
            new() { Id = 31, Name = "New Hampshire", Code = "NH" },
            new() { Id = 32, Name = "New Jersey", Code = "NJ" },
            new() { Id = 33, Name = "New Mexico", Code = "NM" },
            new() { Id = 34, Name = "New York", Code = "NY" },
            new() { Id = 35, Name = "North Carolina", Code = "NC" },
            new() { Id = 36, Name = "North Dakota", Code = "ND" },
            new() { Id = 37, Name = "Ohio", Code = "OH" },
            new() { Id = 38, Name = "Oklahoma", Code = "OK" },
            new() { Id = 39, Name = "Oregon", Code = "OR" },
            new() { Id = 40, Name = "Pennsylvania", Code = "PA" },
            new() { Id = 41, Name = "Puerto Rico", Code = "PR" },
            new() { Id = 42, Name = "Rhode Island", Code = "RI" },
            new() { Id = 43, Name = "South Carolina", Code = "SC" },
            new() { Id = 44, Name = "South Dakota", Code = "SD" },
            new() { Id = 45, Name = "Tennessee", Code = "TN" },
            new() { Id = 46, Name = "Texas", Code = "TX" },
            new() { Id = 47, Name = "Utah", Code = "UT" },
            new() { Id = 48, Name = "Vermont", Code = "VT" },
            new() { Id = 49, Name = "Virginia", Code = "VA" },
            new() { Id = 50, Name = "Washington", Code = "WA" },
            new() { Id = 51, Name = "West Virginia", Code = "WV" },
            new() { Id = 52, Name = "Wisconsin", Code = "WI" },
            new() { Id = 53, Name = "Wyoming", Code = "WY" }
        };

    }
}
