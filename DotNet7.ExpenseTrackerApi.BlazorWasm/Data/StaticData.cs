using DotNet7.ExpenseTrackerApi.BlazorWasm.Models;

namespace DotNet7.ExpenseTrackerApi.BlazorWasm.Data
{
    public static class StaticData
    {
        public static List<KeyValueModel> Gender()
        {
            return new List<KeyValueModel>
            {
                new("0", "Select One"),
                new("Male", "Male"),
                new("Female", "Female"),
            };
        }
    }
}
