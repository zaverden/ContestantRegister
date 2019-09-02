namespace ContestantRegister.Infrastructure.Filter.Contervers
{
    public class NullableIntToNullableBooleanConverter : IFilverValueConverter
    {
        public object Convert(object v)
        {
            var value = (int?)v;
            if (!value.HasValue) return null;

            return value.Value == 1;
        }
    }
}
