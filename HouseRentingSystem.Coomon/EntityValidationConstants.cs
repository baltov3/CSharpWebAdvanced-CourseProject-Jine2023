using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Coomon
{
    public static class EntityValidationConstants
    {
        public static class Category
        {
            public const int CategoryNameMinLenght = 2;
            public const int CategoryNameMaxLenght = 50;
        }
        public static class House
        {
            public const int TitleMinLenght = 10;
            public const int TitleMaxLenght = 50;

            public const int AddressMinLenght = 30;
            public const int AddressMaxLenght = 150;

            public const int DescriptionMinLenght = 50;
            public const int DescriptionMaxLenght = 500;


            public const int ImageUrlMaxLenght = 2048;

            public const string PriceForMonthMinValue = "0";
            public const string PriceForMonthMaxValue = "2000";



        }
        public static class PhoneNumber
        {
            public const int PhoneNumberMinLenght = 7;
            public const int PhoneNumberMaxLenght = 15;
        }
    }
}
