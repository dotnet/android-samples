using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Android.Service.Autofill;
using Android.Util;
using Android.Views;
using AutofillService.Model;
using static Android.Icu.Util.Calendar;

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
                    new AutofillHintProperties(View.AutofillHintEmailAddress, (int) SaveDataType.EmailAddress,
                        PARTITION_EMAIL, new FakeFieldGeneratorForText(View.AutofillHintEmailAddress, "email{0}"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    View.AutofillHintName, new AutofillHintProperties(View.AutofillHintName, (int) SaveDataType.Generic,
                        PARTITION_OTHER, new FakeFieldGeneratorForText(View.AutofillHintName, "name{0}"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    View.AutofillHintUsername, new AutofillHintProperties(View.AutofillHintUsername,
                        (int) SaveDataType.Username,
                        PARTITION_OTHER, new FakeFieldGeneratorForText(View.AutofillHintUsername, "login{0}"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    View.AutofillHintPassword, new AutofillHintProperties(View.AutofillHintPassword,
                        (int) SaveDataType.Password,
                        PARTITION_OTHER, new FakeFieldGeneratorForText(View.AutofillHintPassword, "login{0}"),
                        (int) AutofillType.Text)
                },
                {
                    View.AutofillHintPhone, new AutofillHintProperties(View.AutofillHintPhone,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER, new FakeFieldGeneratorForText(View.AutofillHintPhone, "{0}2345678910"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    View.AutofillHintPostalAddress, new AutofillHintProperties(View.AutofillHintPostalAddress,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(View.AutofillHintPostalAddress,
                            "{0} Fake Ln, Fake, FA, FAA 10001"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    View.AutofillHintPostalCode, new AutofillHintProperties(View.AutofillHintPostalCode,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(View.AutofillHintPostalCode, "1000{0}"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    View.AutofillHintCreditCardNumber, new AutofillHintProperties(View.AutofillHintCreditCardNumber,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(View.AutofillHintCreditCardNumber, "{0}234567"),
                        (int) AutofillType.Text)
                },
                {
                    View.AutofillHintCreditCardSecurityCode, new AutofillHintProperties(View.AutofillHintCreditCardSecurityCode,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(View.AutofillHintCreditCardSecurityCode, "{0}{0}{0}"),
                        (int) AutofillType.Text)
                },
                {
                    View.AutofillHintCreditCardExpirationDate, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationDate,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForDate(View.AutofillHintCreditCardExpirationDate),
                        (int) AutofillType.Date)
                },
                {
                    View.AutofillHintCreditCardExpirationMonth, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationMonth,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorExpirationMonth(View.AutofillHintCreditCardExpirationMonth),
                        (int) AutofillType.Date)
                },
                {
                    View.AutofillHintCreditCardExpirationYear, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationYear,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorExpirationYear(View.AutofillHintCreditCardExpirationYear),
                        (int) AutofillType.Text, (int) AutofillType.List, (int) AutofillType.Date)
                },
                {
                    View.AutofillHintCreditCardExpirationDay, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationDay,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorExpirationDay(View.AutofillHintCreditCardExpirationDay),
                        (int) AutofillType.Text, (int) AutofillType.List, (int) AutofillType.Date)
                },
                {
                    W3cHints.HONORIFIC_PREFIX, new AutofillHintProperties(
                        W3cHints.HONORIFIC_PREFIX,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForListValue(W3cHints.HONORIFIC_PREFIX,
                            new[] {"Miss", "Ms.", "Mr.", "Mx.", "Sr.", "Dr.", "Lady", "Lord"}),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.GIVEN_NAME, new AutofillHintProperties(
                        W3cHints.GIVEN_NAME,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.GIVEN_NAME, "name{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.ADDITIONAL_NAME, new AutofillHintProperties(
                        W3cHints.ADDITIONAL_NAME,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.ADDITIONAL_NAME, "addtlname{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.FAMILY_NAME, new AutofillHintProperties(
                        W3cHints.FAMILY_NAME,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.FAMILY_NAME, "famname{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.HONORIFIC_SUFFIX, new AutofillHintProperties(
                        W3cHints.HONORIFIC_SUFFIX,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForListValue(W3cHints.HONORIFIC_SUFFIX,
                            new[] {"san", "kun", "chan", "sama"}),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.NEW_PASSWORD, new AutofillHintProperties(
                        W3cHints.NEW_PASSWORD,
                        (int) SaveDataType.Password,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.NEW_PASSWORD, "login{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.CURRENT_PASSWORD, new AutofillHintProperties(
                        View.AutofillHintPassword,
                        (int) SaveDataType.Password,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(View.AutofillHintPassword, "login{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.ORGANIZATION_TITLE, new AutofillHintProperties(
                        W3cHints.ORGANIZATION_TITLE,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.ORGANIZATION_TITLE, "org{0}"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.ORGANIZATION, new AutofillHintProperties(
                        W3cHints.ORGANIZATION,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.ORGANIZATION, "org{0}"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.STREET_ADDRESS, new AutofillHintProperties(
                        W3cHints.STREET_ADDRESS,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(W3cHints.STREET_ADDRESS, "{0} Fake Ln, Fake, FA, FAA 10001"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LINE1, new AutofillHintProperties(
                        W3cHints.ADDRESS_LINE1,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(W3cHints.ADDRESS_LINE1, "{0} Fake Ln"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LINE2, new AutofillHintProperties(
                        W3cHints.ADDRESS_LINE2,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(W3cHints.ADDRESS_LINE2, "Apt. {0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LINE3, new AutofillHintProperties(
                        W3cHints.ADDRESS_LINE3,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(W3cHints.ADDRESS_LINE3, "FA{0}, FA, FAA"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LEVEL4, new AutofillHintProperties(
                        W3cHints.ADDRESS_LEVEL4,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGenerator(W3cHints.ADDRESS_LEVEL4),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LEVEL3, new AutofillHintProperties(
                        W3cHints.ADDRESS_LEVEL3,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGenerator(W3cHints.ADDRESS_LEVEL3),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LEVEL2, new AutofillHintProperties(
                        W3cHints.ADDRESS_LEVEL2,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGenerator(W3cHints.ADDRESS_LEVEL2),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.ADDRESS_LEVEL1, new AutofillHintProperties(
                        W3cHints.ADDRESS_LEVEL1,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGenerator(W3cHints.ADDRESS_LEVEL1),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.COUNTRY, new AutofillHintProperties(
                        W3cHints.COUNTRY,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGenerator(W3cHints.COUNTRY),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.COUNTRY_NAME, new AutofillHintProperties(
                        W3cHints.COUNTRY_NAME,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForListValue(W3cHints.COUNTRY_NAME, new[] {"USA", "Mexico", "Canada"}),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.POSTAL_CODE, new AutofillHintProperties(
                        W3cHints.POSTAL_CODE,
                        (int) SaveDataType.Address,
                        PARTITION_ADDRESS,
                        new FakeFieldGeneratorForText(W3cHints.POSTAL_CODE, "{0}{0}{0}{0}{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.CC_NAME, new AutofillHintProperties(
                        W3cHints.CC_NAME,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(W3cHints.CC_NAME, "firstname{0}lastname{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.CC_GIVEN_NAME, new AutofillHintProperties(
                        W3cHints.CC_GIVEN_NAME,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(W3cHints.CC_GIVEN_NAME, "givenname{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.CC_ADDITIONAL_NAME, new AutofillHintProperties(
                        W3cHints.CC_ADDITIONAL_NAME,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(W3cHints.CC_ADDITIONAL_NAME, "addtlname{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.CC_FAMILY_NAME, new AutofillHintProperties(
                        W3cHints.CC_FAMILY_NAME,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(W3cHints.CC_FAMILY_NAME, "familyname{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.CC_NUMBER, new AutofillHintProperties(
                        View.AutofillHintCreditCardNumber,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(View.AutofillHintCreditCardNumber, "{0}234567"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.CC_EXPIRATION, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationDate,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForDate(View.AutofillHintCreditCardExpirationDate),
                        (int) AutofillType.Date)
                },
                {
                    W3cHints.CC_EXPIRATION_MONTH, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationMonth,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorExpirationMonth(View.AutofillHintCreditCardExpirationMonth),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.CC_EXPIRATION_YEAR, new AutofillHintProperties(
                        View.AutofillHintCreditCardExpirationYear,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorExpirationYear(View.AutofillHintCreditCardExpirationYear),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.CC_CSC, new AutofillHintProperties(
                        View.AutofillHintCreditCardSecurityCode,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(View.AutofillHintCreditCardSecurityCode, "{0}{0}{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.CC_TYPE, new AutofillHintProperties(
                        W3cHints.CC_TYPE,
                        (int) SaveDataType.CreditCard,
                        PARTITION_CREDIT_CARD,
                        new FakeFieldGeneratorForText(W3cHints.CC_TYPE, "type{0}"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.TRANSACTION_CURRENCY, new AutofillHintProperties(
                        W3cHints.TRANSACTION_CURRENCY,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForListValue(W3cHints.TRANSACTION_CURRENCY,
                            new[] {"USD", "CAD", "KYD", "CRC"}),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.TRANSACTION_AMOUNT, new AutofillHintProperties(
                        W3cHints.TRANSACTION_AMOUNT,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForNumberMultiply(W3cHints.TRANSACTION_AMOUNT, 100),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.LANGUAGE, new AutofillHintProperties(
                        W3cHints.LANGUAGE,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForListValue(W3cHints.LANGUAGE,
                            new[] {"Bulgarian", "Croatian", "Czech", "Danish", "Dutch", "English", "Estonian"}),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.BDAY, new AutofillHintProperties(
                        W3cHints.BDAY,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForBirthDay(W3cHints.BDAY),
                        (int) AutofillType.Date)
                },
                {
                    W3cHints.BDAY_DAY, new AutofillHintProperties(
                        W3cHints.BDAY_DAY,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForNumberDivide(W3cHints.BDAY_DAY, 27),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.BDAY_MONTH, new AutofillHintProperties(
                        W3cHints.BDAY_MONTH,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForNumberDivide(W3cHints.BDAY_MONTH, 12),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.BDAY_YEAR, new AutofillHintProperties(
                        W3cHints.BDAY_YEAR,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorBdayYear(W3cHints.BDAY_YEAR),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.SEX, new AutofillHintProperties(
                        W3cHints.SEX,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.SEX, "Other"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.URL, new AutofillHintProperties(
                        W3cHints.URL,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.URL, "http://google.com"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.PHOTO, new AutofillHintProperties(
                        W3cHints.PHOTO,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGeneratorForText(W3cHints.PHOTO, "photo{0}.jpg"),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.PREFIX_SECTION, new AutofillHintProperties(
                        W3cHints.PREFIX_SECTION,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.PREFIX_SECTION),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.SHIPPING, new AutofillHintProperties(
                        W3cHints.SHIPPING,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.SHIPPING),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.BILLING, new AutofillHintProperties(
                        W3cHints.BILLING,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.BILLING),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.PREFIX_HOME, new AutofillHintProperties(
                        W3cHints.PREFIX_HOME,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.PREFIX_HOME),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.PREFIX_WORK, new AutofillHintProperties(
                        W3cHints.PREFIX_WORK,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.PREFIX_WORK),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.PREFIX_FAX, new AutofillHintProperties(
                        W3cHints.PREFIX_FAX,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.PREFIX_FAX),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.PREFIX_PAGER, new AutofillHintProperties(
                        W3cHints.PREFIX_PAGER,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.PREFIX_PAGER),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.TEL, new AutofillHintProperties(
                        W3cHints.TEL,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.TEL_COUNTRY_CODE, new AutofillHintProperties(
                        W3cHints.TEL_COUNTRY_CODE,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_COUNTRY_CODE),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.TEL_NATIONAL, new AutofillHintProperties(
                        W3cHints.TEL_NATIONAL,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_NATIONAL),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.TEL_AREA_CODE, new AutofillHintProperties(
                        W3cHints.TEL_AREA_CODE,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_AREA_CODE),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.TEL_LOCAL, new AutofillHintProperties(
                        W3cHints.TEL_LOCAL,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_LOCAL),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.TEL_LOCAL_PREFIX, new AutofillHintProperties(
                        W3cHints.TEL_LOCAL_PREFIX,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_LOCAL_PREFIX),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.TEL_LOCAL_SUFFIX, new AutofillHintProperties(
                        W3cHints.TEL_LOCAL_SUFFIX,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_LOCAL_SUFFIX),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.TEL_EXTENSION, new AutofillHintProperties(
                        W3cHints.TEL_EXTENSION,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.TEL_EXTENSION),
                        (int) AutofillType.Text, (int) AutofillType.List)
                },
                {
                    W3cHints.EMAIL, new AutofillHintProperties(
                        View.AutofillHintEmailAddress,
                        (int) SaveDataType.Generic,
                        PARTITION_EMAIL,
                        new FakeFieldGeneratorForText(W3cHints.EMAIL, "email{0}"),
                        (int) AutofillType.Text)
                },
                {
                    W3cHints.IMPP, new AutofillHintProperties(
                        W3cHints.IMPP,
                        (int) SaveDataType.Generic,
                        PARTITION_OTHER,
                        new FakeFieldGenerator(W3cHints.IMPP),
                        (int) AutofillType.Text, (int) AutofillType.List)
                }
            }.ToImmutableDictionary();

        private AutofillHints()
        {
        }

        public static bool IsValidTypeForHints(string[] hints, int type)
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

        public static int GetSaveTypeForHints(string[] hints)
        {
            var saveType = 0;
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
                var calendar = Instance;
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
                var calendar = Instance;
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
                var calendar = Instance;
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
                var calendar = Instance;
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
                int year = Instance.Get(Year) - seed * 10;
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
                var calendar = Instance;
                calendar.Set(Year, calendar.Get(Year) - seed * 10);
                calendar.Set(Month, seed % 12);
                calendar.Set(Date, seed % 27);
                filledAutofillField.SetDateValue(calendar.TimeInMillis);
                return filledAutofillField;
            }
        }
    }
}