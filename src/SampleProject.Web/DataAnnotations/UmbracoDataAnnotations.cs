using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

//Helping reference: https://github.com/mono/aspnetwebstack/blob/master/src/System.Web.Mvc/CompareAttribute.cs

namespace Umbraco.Web.DataAnnotations
{
    public class UmbracoDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string resourceName;

        public UmbracoDisplayNameAttribute(string resourceName)
            : base()
        {
            this.resourceName = resourceName;
        }

        public override string DisplayName
        {
            get
            {
                return UmbracoDictionary.GetDictionaryValue(resourceName);
            }
        }
    }

    public class UmbracoRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        public string ResourceName { get; set; } = "RequiredError";

        public UmbracoRequiredAttribute() :
            base()
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ErrorMessage = UmbracoDictionary.GetDictionaryValue(ResourceName);
            yield return new ModelClientValidationRequiredRule(FormatErrorMessage(metadata.GetDisplayName()));
        }
    }

    public class UmbracoStringLengthAttribute : StringLengthAttribute, IClientValidatable
    {
        public UmbracoStringLengthAttribute(int maximumLength)
            : base(maximumLength)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            if (MinimumLength > 0)
            {
                ErrorMessage = UmbracoDictionary.GetDictionaryValue("MinMaxLengthError");
            }
            else
            {
                ErrorMessage = UmbracoDictionary.GetDictionaryValue("MaxLengthError");
            }

            yield return
                new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), MinimumLength, MaximumLength);
        }

        public UmbracoStringLengthAttribute(int maximumLength, string resourceName)
            : base(maximumLength)
        {
            ErrorMessage = UmbracoDictionary.GetDictionaryValue(resourceName);
        }
    }

    public class UmbracoMaxLengthAttribute : MaxLengthAttribute, IClientValidatable
    {
        public UmbracoMaxLengthAttribute(int length)
            : base(length)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ErrorMessage = UmbracoDictionary.GetDictionaryValue("MaxLengthError");
            yield return
                new ModelClientValidationMaxLengthRule(FormatErrorMessage(metadata.GetDisplayName()), Length);
        }

        public UmbracoMaxLengthAttribute(int length, string resourceName)
            : base(length)
        {
            ErrorMessage = UmbracoDictionary.GetDictionaryValue(resourceName);
        }
    }

    public class UmbracoMinLengthAttribute : MinLengthAttribute, IClientValidatable
    {
        public UmbracoMinLengthAttribute(int length)
            : base(length)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ErrorMessage = UmbracoDictionary.GetDictionaryValue("MinLengthError");
            yield return
                new ModelClientValidationMinLengthRule(FormatErrorMessage(metadata.GetDisplayName()), Length);
        }

        public UmbracoMinLengthAttribute(int length, string resourceName)
            : base(length)
        {
            ErrorMessage = UmbracoDictionary.GetDictionaryValue(resourceName);
        }
    }


    public class UmbracoRegularExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public string ResourceName { get; set; } = "RegexError";

        public UmbracoRegularExpressionAttribute(string pattern)
            : base(pattern)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ErrorMessage = UmbracoDictionary.GetDictionaryValue(ResourceName);
            yield return new ModelClientValidationRegexRule(FormatErrorMessage(metadata.GetDisplayName()), Pattern);
        }
    }

    public class UmbracoCompareAttribute : System.ComponentModel.DataAnnotations.CompareAttribute, IClientValidatable
    {
        public new string OtherPropertyDisplayName { get; internal set; }

        public UmbracoCompareAttribute(string otherProperty)
            : base(otherProperty)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherPropertyDisplayName ?? OtherProperty);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            if (metadata.ContainerType != null)
            {
                if (OtherPropertyDisplayName == null)
                {
                    OtherPropertyDisplayName = ModelMetadataProviders.Current.GetMetadataForProperty(() => metadata.Model, metadata.ContainerType, OtherProperty).GetDisplayName();
                }
            }

            ErrorMessage = UmbracoDictionary.GetDictionaryValue("CompareError");
            yield return new ModelClientValidationEqualToRule(FormatErrorMessage(metadata.GetDisplayName()), OtherProperty);
        }
    }

    public class UmbracoEmailAddress : RegularExpressionAttribute, IClientValidatable
    {
        public string ResourceName { get; set; } = "EmailError";

        private static new string Pattern { get; set; } = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        public UmbracoEmailAddress()
            : base(Pattern)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ErrorMessage = UmbracoDictionary.GetDictionaryValue(ResourceName);
            yield return new ModelClientValidationRegexRule(FormatErrorMessage(metadata.GetDisplayName()), Pattern);
        }
    }

    public class UmbracoDictionary
    {
        private static UmbracoHelper _helper;

        private static UmbracoHelper Helper
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new UmbracoHelper();
                }
                return _helper;
            }
        }

        public static string GetDictionaryValue(string resourceKey)
        {
            string key = Helper.GetDictionaryValue(resourceKey);
            if (!string.IsNullOrEmpty(key))
                return key;
            return resourceKey; // Fallback with the key name
        }
    }
}