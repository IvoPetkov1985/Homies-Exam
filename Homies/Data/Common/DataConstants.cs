namespace Homies.Data.Common
{
    public static class DataConstants
    {
        // Event constants:
        public const int EventNameMinLength = 5;
        public const int EventNameMaxLength = 20;

        public const int DescriptionMinLength = 15;
        public const int DescriptionMaxLength = 150;

        public const string DateTimeFormat = "yyyy-MM-dd H:mm";
        public const string DateTimeErrorMsg = "Date and time should be in format 'yyyy-MM-dd H:mm'.";
        public const string DateTimeRegex = @"^\d{4}-\d{2}-\d{2} \d{1,2}:\d{2}$";
        public const string DateStartAfterTheEnd = "End should not be before the start.";
        public const string DateTimeInvalid = "Invalid date and time input.";

        // Type constants:
        public const int TypeNameMinLength = 5;
        public const int TypeNameMaxLength = 15;

        public const string TypeInvalidErrorMsg = "This type does not exist.";

        // Names of actions and controllers:
        public const string AllAction = "All";
        public const string EventContr = "Event";
    }
}
