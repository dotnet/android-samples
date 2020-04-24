using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Android.Service.Autofill;
using Android.Util;
using Android.Views;
using AutofillService.Model;
using static Android.Icu.Util.CalendarField;

namespace AutofillService
{
    public class AutofillHints
    {
        public static int PARTITION_OTHER = 0;
        public static int PARTITION_ADDRESS = 1;
        public static int PARTITION_EMAIL = 2;
        public static int PARTITION_CREDIT_CARD = 3;
        public static int[] PARTITIONS = {PARTITION_OTHER, PARTITION_ADDRESS, PARTITION_EMAIL, PARTITION_CREDIT_CARD};

        //TODO: finish building fake data for all hints.
        private static ImmutableDictionary<string, AutofillHintProperties> sValidHints =
            new Dictionary<string, AutofillHintProperties>()
            {
                {
                    View.AutofillHintEmailAddress,
                    new AutofillHintProperties(View.AutofillHintEmailAddress, SaveDataType.EmailAddress,
                        PARTITION_EMAIL, new FakeFieldGeneratorForText(View.AutofillHintEmailAddress, "email{0}"),
                        AutofillType.Text, AutofillType.List)
                },
                {
                    View.AutofillHintName, new AutofillHintProperties(View.AutofillHintName,  SaveDataType.Generic,
                        PARTITION_OTHER, new FakeFieldGeneratorForText(View.AutofillHintName, "name{0}"),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    View.AutofillHintUsername, new AutofillHintProperties(View.AutofillHintUsername,
                        SaveDataType.Username,
                        PARTITION_OTHER, new FakeFieldGeneratorForText(View.AutofillHintUsername, "login{0}"),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    View.AutofillHintPassword, new AutofillHintProperties(View.AutofillHintPassword,
                         SaveDataType.Password,
                        PARTITION_OTHER, new FakeFieldGeneratorForText(View.AutofillHintPassword, "login{0}"),
                         AutofillType.Text)
                },
                {
                    View.AutofillHintPhone, new AutofillHintProperties(View.AutofillHintPhone,
                        SaveDataType.Generic,
                        PARTITION_OTHER, new FakeFieldGeneratorForText(View.AutofillHintPhone, "{0}2345678910"),
                         AutofillType.Text, AutofillType.List)
                },
                {
                    View.AutofillHintPostalAddress, new AutofillHintProperties(View.AutofillHintPostalAddress,
                        SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(View.AutofillHintPostalAddress,
                            "{0} Fake Ln, Fake, FA, FAA 10001"),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    View.AutofillHintPostalCode, new AutofillHintProperties(View.AutofillHintPostalCode,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(View.AutofillHintPostalCode, "1000{0}"),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    View.AutofillHintCreditCardNumber, new AutofillHintProperties(View.AutofillHintCreditCardNumber,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(View.AutofillHintCreditCardNumber, "{0}234567"),
                         AutofillType.Text)
                },
                {
                    View.AutofillHintCreditCardSecurityCode, new AutofillHintProperties(View.AutofillHintCreditCardSecurityCode,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(View.AutofillHintCreditCardSecurityCode, "{0}{0}{0}"),
                         AutofillType.Text)
                },
                {
                    View.AutofillHintCreditCardExpirationDate, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationDate,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForDate(View.AutofillHintCreditCardExpirationDate),
                         AutofillType.Date)
                },
                {
                    View.AutofillHintCreditCardExpirationMonth, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationMonth,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorExpirationMonth(View.AutofillHintCreditCardExpirationMonth),
                         AutofillType.Date)
                },
                {
                    View.AutofillHintCreditCardExpirationYear, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationYear,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorExpirationYear(View.AutofillHintCreditCardExpirationYear),
                         AutofillType.Text,  AutofillType.List,  AutofillType.Date)
                },
                {
                    View.AutofillHintCreditCardExpirationDay, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationDay,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorExpirationDay(View.AutofillHintCreditCardExpirationDay),
                         AutofillType.Text,  AutofillType.List,  AutofillType.Date)
                },
                {
                    W3cHints.HONORIFIC_PREFIX, new AutofillHintProperties(
                        W3cHints.HONORIFIC_PREFIX,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForListValue(W3cHints.HONORIFIC_PREFIX,
                            new[] {"Miss", "Ms.", "Mr.", "Mx.", "Sr.", "Dr.", "Lady", "Lord"}),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.GIVEN_NAME, new AutofillHintProperties(
                        W3cHints.GIVEN_NAME,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.GIVEN_NAME, "name{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.ADDITIONAL_NAME, new AutofillHintProperties(
                        W3cHints.ADDITIONAL_NAME,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.ADDITIONAL_NAME, "addtlname{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.FAMILY_NAME, new AutofillHintProperties(
                        W3cHints.FAMILY_NAME,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.FAMILY_NAME, "famname{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.HONORIFIC_SUFFIX, new AutofillHintProperties(
                        W3cHints.HONORIFIC_SUFFIX,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForListValue(W3cHints.HONORIFIC_SUFFIX,
                            new[] {"san", "kun", "chan", "sama"}),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.NEW_PASSWORD, new AutofillHintProperties(
                        W3cHints.NEW_PASSWORD,
                         SaveDataType.Password,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.NEW_PASSWORD, "login{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.CURRENT_PASSWORD, new AutofillHintProperties(
                        View.AutofillHintPassword,
                         SaveDataType.Password,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(View.AutofillHintPassword, "login{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.ORGANIZATION_TITLE, new AutofillHintProperties(
                        W3cHints.ORGANIZATION_TITLE,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.ORGANIZATION_TITLE, "org{0}"),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.ORGANIZATION, new AutofillHintProperties(
                        W3cHints.ORGANIZATION,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.ORGANIZATION, "org{0}"),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.STREET_ADDRESS, new AutofillHintProperties(
                        W3cHints.STREET_ADDRESS,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(W3cHints.STREET_ADDRESS, "{0} Fake Ln, Fake, FA, FAA 10001"),
                         AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LINE1, new AutofillHintProperties(
                        W3cHints.ADDRESS_LINE1,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(W3cHints.ADDRESS_LINE1, "{0} Fake Ln"),
                         AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LINE2, new AutofillHintProperties(
                        W3cHints.ADDRESS_LINE2,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(W3cHints.ADDRESS_LINE2, "Apt. {0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LINE3, new AutofillHintProperties(
                        W3cHints.ADDRESS_LINE3,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(W3cHints.ADDRESS_LINE3, "FA{0}, FA, FAA"),
                         AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LEVEL4, new AutofillHintProperties(
                        W3cHints.ADDRESS_LEVEL4,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGenerator(W3cHints.ADDRESS_LEVEL4),
                         AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LEVEL3, new AutofillHintProperties(
                        W3cHints.ADDRESS_LEVEL3,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGenerator(W3cHints.ADDRESS_LEVEL3),
                         AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LEVEL2, new AutofillHintProperties(
                        W3cHints.ADDRESS_LEVEL2,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGenerator(W3cHints.ADDRESS_LEVEL2),
                         AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LEVEL1, new AutofillHintProperties(
                        W3cHints.ADDRESS_LEVEL1,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGenerator(W3cHints.ADDRESS_LEVEL1),
                         AutofillType.Text)
                },
                {
                    W3cHints.COUNTRY, new AutofillHintProperties(
                        W3cHints.COUNTRY,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGenerator(W3cHints.COUNTRY),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.COUNTRY_NAME, new AutofillHintProperties(
                        W3cHints.COUNTRY_NAME,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForListValue(W3cHints.COUNTRY_NAME, new[] {"USA", "Mexico", "Canada"}),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.POSTAL_CODE, new AutofillHintProperties(
                        W3cHints.POSTAL_CODE,
                         SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(W3cHints.POSTAL_CODE, "{0}{0}{0}{0}{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.CC_NAME, new AutofillHintProperties(
                        W3cHints.CC_NAME,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(W3cHints.CC_NAME, "firstname{0}lastname{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.CC_GIVEN_NAME, new AutofillHintProperties(
                        W3cHints.CC_GIVEN_NAME,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(W3cHints.CC_GIVEN_NAME, "givenname{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.CC_ADDITIONAL_NAME, new AutofillHintProperties(
                        W3cHints.CC_ADDITIONAL_NAME,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(W3cHints.CC_ADDITIONAL_NAME, "addtlname{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.CC_FAMILY_NAME, new AutofillHintProperties(
                        W3cHints.CC_FAMILY_NAME,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(W3cHints.CC_FAMILY_NAME, "familyname{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.CC_NUMBER, new AutofillHintProperties(
                        View.AutofillHintCreditCardNumber,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(View.AutofillHintCreditCardNumber, "{0}234567"),
                         AutofillType.Text)
                },
                {
                    W3cHints.CC_EXPIRATION, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationDate,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForDate(View.AutofillHintCreditCardExpirationDate),
                         AutofillType.Date)
                },
                {
                    W3cHints.CC_EXPIRATION_MONTH, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationMonth,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorExpirationMonth(View.AutofillHintCreditCardExpirationMonth),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.CC_EXPIRATION_YEAR, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationYear,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorExpirationYear(View.AutofillHintCreditCardExpirationYear),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.CC_CSC, new AutofillHintProperties(
                        View.AutofillHintCreditCardSecurityCode,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(View.AutofillHintCreditCardSecurityCode, "{0}{0}{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.CC_TYPE, new AutofillHintProperties(
                        W3cHints.CC_TYPE,
                         SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(W3cHints.CC_TYPE, "type{0}"),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.TRANSACTION_CURRENCY, new AutofillHintProperties(
                        W3cHints.TRANSACTION_CURRENCY,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForListValue(W3cHints.TRANSACTION_CURRENCY,
                            new[] {"USD", "CAD", "KYD", "CRC"}),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.TRANSACTION_AMOUNT, new AutofillHintProperties(
                        W3cHints.TRANSACTION_AMOUNT,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForNumberMultiply(W3cHints.TRANSACTION_AMOUNT, 100),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.LANGUAGE, new AutofillHintProperties(
                        W3cHints.LANGUAGE,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForListValue(W3cHints.LANGUAGE,
                            new[] {"Bulgarian", "Croatian", "Czech", "Danish", "Dutch", "English", "Estonian"}),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.BDAY, new AutofillHintProperties(
                        W3cHints.BDAY,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForBirthDay(W3cHints.BDAY),
                         AutofillType.Date)
                },
                {
                    W3cHints.BDAY_DAY, new AutofillHintProperties(
                        W3cHints.BDAY_DAY,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForNumberDivide(W3cHints.BDAY_DAY, 27),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.BDAY_MONTH, new AutofillHintProperties(
                        W3cHints.BDAY_MONTH,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForNumberDivide(W3cHints.BDAY_MONTH, 12),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.BDAY_YEAR, new AutofillHintProperties(
                        W3cHints.BDAY_YEAR,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorBdayYear(W3cHints.BDAY_YEAR),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.SEX, new AutofillHintProperties(
                        W3cHints.SEX,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.SEX, "Other"),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.URL, new AutofillHintProperties(
                        W3cHints.URL,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.URL, "http://google.com"),
                         AutofillType.Text)
                },
                {
                    W3cHints.PHOTO, new AutofillHintProperties(
                        W3cHints.PHOTO,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.PHOTO, "photo{0}.jpg"),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.PREFIX_SECTION, new AutofillHintProperties(
                        W3cHints.PREFIX_SECTION,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.PREFIX_SECTION),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.SHIPPING, new AutofillHintProperties(
                        W3cHints.SHIPPING,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.SHIPPING),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.BILLING, new AutofillHintProperties(
                        W3cHints.BILLING,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.BILLING),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.PREFIX_HOME, new AutofillHintProperties(
                        W3cHints.PREFIX_HOME,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.PREFIX_HOME),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.PREFIX_WORK, new AutofillHintProperties(
                        W3cHints.PREFIX_WORK,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.PREFIX_WORK),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.PREFIX_FAX, new AutofillHintProperties(
                        W3cHints.PREFIX_FAX,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.PREFIX_FAX),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.PREFIX_PAGER, new AutofillHintProperties(
                        W3cHints.PREFIX_PAGER,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.PREFIX_PAGER),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.TEL, new AutofillHintProperties(
                        W3cHints.TEL,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.TEL_COUNTRY_CODE, new AutofillHintProperties(
                        W3cHints.TEL_COUNTRY_CODE,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_COUNTRY_CODE),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.TEL_NATIONAL, new AutofillHintProperties(
                        W3cHints.TEL_NATIONAL,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_NATIONAL),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.TEL_AREA_CODE, new AutofillHintProperties(
                        W3cHints.TEL_AREA_CODE,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_AREA_CODE),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.TEL_LOCAL, new AutofillHintProperties(
                        W3cHints.TEL_LOCAL,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_LOCAL),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.TEL_LOCAL_PREFIX, new AutofillHintProperties(
                        W3cHints.TEL_LOCAL_PREFIX,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_LOCAL_PREFIX),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.TEL_LOCAL_SUFFIX, new AutofillHintProperties(
                        W3cHints.TEL_LOCAL_SUFFIX,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_LOCAL_SUFFIX),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.TEL_EXTENSION, new AutofillHintProperties(
                        W3cHints.TEL_EXTENSION,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_EXTENSION),
                         AutofillType.Text,  AutofillType.List)
                },
                {
                    W3cHints.EMAIL, new AutofillHintProperties(
                        View.AutofillHintEmailAddress,
                         SaveDataType.Generic,
                        PARTITION_EMAIL,
                        new FakeFieldGeneratorForText(W3cHints.EMAIL, "email{0}"),
                         AutofillType.Text)
                },
                {
                    W3cHints.IMPP, new AutofillHintProperties(
                        W3cHints.IMPP,
                         SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.IMPP),
                         AutofillType.Text,  AutofillType.List)
                }
            }.ToImmutableDictionary();

        private AutofillHints()
        {
        }

        public static bool IsValidTypeForHints(string[] hints, AutofillType type)
        {
            if (hints != null)
            {
                foreach (var hint in hints)
                {
                    if (hint != null && sValidHints.ContainsKey(hint))
                    {
                        var valid = sValidHints[hint].IsValidType(type);
                        if (valid)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsValidHint(string hint)
        {
            return sValidHints.ContainsKey(hint);
        }

        public static SaveDataType GetSaveTypeForHints(string[] hints)
        {
            SaveDataType saveType = 0;
            if (hints != null)
            {
                foreach (string hint in hints)
                {
                    if (hint != null && sValidHints.ContainsKey(hint))
                    {
                        saveType |= sValidHints[hint].GetSaveType();
                    }
                }
            }

            return saveType;
        }

        public static FilledAutofillField GetFakeField(string hint, int seed)
        {
            return sValidHints[hint].GenerateFakeField(seed);
        }

        public static FilledAutofillFieldCollection GetFakeFieldCollection(int partition, int seed)
        {
            var filledAutofillFieldCollection = new FilledAutofillFieldCollection();
            foreach (var hint in sValidHints.Keys)
            {
                if (hint != null && sValidHints[hint].GetPartition() == partition)
                {
                    var fakeField = GetFakeField(hint, seed);
                    filledAutofillFieldCollection.Add(fakeField);
                }
            }

            return filledAutofillFieldCollection;
        }

        private static string GetStoredHintName(string hint)
        {
            return sValidHints[hint].GetAutofillHint();
        }

        public static void ConvertToStoredHintNames(string[] hints)
        {
            for (int i = 0; i < hints.Length; i++)
            {
                hints[i] = GetStoredHintName(hints[i]);
            }
        }

        private static string[] DayRange()
        {
            var days = new string[27];
            for (int i = 0; i < days.Length; i++)
            {
                days[i] = i.ToString();
            }

            return days;
        }

        private static string[] MonthRange()
        {
            string[] months = new string[12];
            for (int i = 0; i < months.Length; i++)
            {
                months[i] = i.ToString();
            }

            return months;
        }

        public static string[] FilterForSupportedHints(string[] hints)
        {
            var filteredHints = new string[hints.Length];
            var i = 0;
            foreach (var hint in hints)
            {
                if (IsValidHint(hint))
                {
                    filteredHints[i++] = hint;
                }
                else
                {
                    Log.Warn(CommonUtil.TAG, "Invalid autofill hint: " + hint);
                }
            }

            if (i == 0)
            {
                return null;
            }

            var finalFilteredHints = new string[i];
            Array.Copy(filteredHints, 0, finalFilteredHints, 0, i);
            return finalFilteredHints;
        }

        public class FakeFieldGeneratorForText : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;
            public string Text;

            public FakeFieldGeneratorForText(string type, string text)
            {
                Type = type;
                Text = text;
            }

            public FilledAutofillField Generate(int seed)
            {
                var filledAutofillField = new FilledAutofillField(Type);
                filledAutofillField.SetTextValue(string.Format(Text, seed));
                return filledAutofillField;
            }
        }

        public class FakeFieldGeneratorForNumberMultiply : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;
            public int Number;

            public FakeFieldGeneratorForNumberMultiply(string type, int number)
            {
                Type = type;
                Number = number;
            }

            public FilledAutofillField Generate(int seed)
            {
                var filledAutofillField = new FilledAutofillField(Type);
                filledAutofillField.SetTextValue((seed * Number).ToString());
                return filledAutofillField;
            }
        }

        public class FakeFieldGeneratorForNumberDivide : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;
            public int Number;

            public FakeFieldGeneratorForNumberDivide(string type, int number)
            {
                Type = type;
                Number = number;
            }

            public FilledAutofillField Generate(int seed)
            {
                var filledAutofillField = new FilledAutofillField(Type);
                filledAutofillField.SetTextValue((seed % Number).ToString());
                return filledAutofillField;
            }
        }

        public class FakeFieldGenerator : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;


            public FakeFieldGenerator(string type)
            {
                Type = type;
            }

            public FilledAutofillField Generate(int seed)
            {
                var filledAutofillField = new FilledAutofillField(Type);
                return filledAutofillField;
            }
        }

        public class FakeFieldGeneratorForListValue : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;
            public string[] Array;

            public FakeFieldGeneratorForListValue(string type, string[] array)
            {
                Type = type;
                Array = array;
            }

            public FilledAutofillField Generate(int seed)
            {
                var filledAutofillField = new FilledAutofillField(Type);
                filledAutofillField.SetListValue(Array, seed % Array.Length);
                return filledAutofillField;
            }
        }

        public class FakeFieldGeneratorForDate : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;

            public FakeFieldGeneratorForDate(string type)
            {
                Type = type;
            }

            public FilledAutofillField Generate(int seed)
            {
                var filledAutofillField = new FilledAutofillField(Type);
                var calendar = Android.Icu.Util.Calendar.Instance;
                calendar.Set(Year, calendar.Get(Year) + seed);
                filledAutofillField.SetDateValue(calendar.TimeInMillis);
                return filledAutofillField;
            }
        }

        public class FakeFieldGeneratorExpirationMonth : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;

            public FakeFieldGeneratorExpirationMonth(string type)
            {
                Type = type;
            }

            public FilledAutofillField Generate(int seed)
            {
                var months = MonthRange();
                int month = seed % months.Length;
                var calendar = Android.Icu.Util.Calendar.Instance;
                calendar.Set(Month, month);
                var filledAutofillField = new FilledAutofillField(Type);
                filledAutofillField.SetListValue(months, month);
                filledAutofillField.SetTextValue(month.ToString());
                filledAutofillField.SetDateValue(calendar.TimeInMillis);
                return filledAutofillField;
            }
        }

        public class FakeFieldGeneratorExpirationYear : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;

            public FakeFieldGeneratorExpirationYear(string type)
            {
                Type = type;
            }

            public FilledAutofillField Generate(int seed)
            {
                FilledAutofillField filledAutofillField = new FilledAutofillField(Type);
                var calendar = Android.Icu.Util.Calendar.Instance;
                int expYear = calendar.Get(Year) + seed;
                calendar.Set(Year, expYear);
                filledAutofillField.SetDateValue(calendar.TimeInMillis);
                filledAutofillField.SetTextValue(expYear.ToString());
                return filledAutofillField;
            }
        }

        public class FakeFieldGeneratorExpirationDay : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;

            public FakeFieldGeneratorExpirationDay(string type)
            {
                Type = type;
            }

            public FilledAutofillField Generate(int seed)
            {
                var days = DayRange();
                int day = seed % days.Length;
                var filledAutofillField = new FilledAutofillField(Type);
                var calendar = Android.Icu.Util.Calendar.Instance;
                calendar.Set(Date, day);
                filledAutofillField.SetListValue(days, day);
                filledAutofillField.SetTextValue(day.ToString());
                filledAutofillField.SetDateValue(calendar.TimeInMillis);
                return filledAutofillField;
            }
        }

        public class FakeFieldGeneratorBdayYear : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;

            public FakeFieldGeneratorBdayYear(string type)
            {
                Type = type;
            }

            public FilledAutofillField Generate(int seed)
            {
                var filledAutofillField = new FilledAutofillField(Type);
                int year = Android.Icu.Util.Calendar.Instance.Get(Year) - seed * 10;
                filledAutofillField.SetTextValue("" + year);
                return filledAutofillField;
            }
        }

        public class FakeFieldGeneratorForBirthDay : Java.Lang.Object, IFakeFieldGenerator
        {
            public string Type;

            public FakeFieldGeneratorForBirthDay(string type)
            {
                Type = type;
            }

            public FilledAutofillField Generate(int seed)
            {
                var filledAutofillField = new FilledAutofillField(Type);
                var calendar = Android.Icu.Util.Calendar.Instance;
                calendar.Set(Year, calendar.Get(Year) - seed * 10);
                calendar.Set(Month, seed % 12);
                calendar.Set(Date, seed % 27);
                filledAutofillField.SetDateValue(calendar.TimeInMillis);
                return filledAutofillField;
            }
        }
    }
}