using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web.Security;
using System.Web.Util;
using Umbraco.Web.Security.Providers;

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
                return UmbracoDictionary.Value(resourceName);
            }
        }
    }

    /// <summary>
    /// Specifies that a data field value is required.
    /// </summary>
    public class UmbracoRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        public string ResourceName { get; set; } = "RequiredError";

        public UmbracoRequiredAttribute() :
            base()
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ErrorMessage = UmbracoDictionary.Value(ResourceName);
            yield return new ModelClientValidationRequiredRule(FormatErrorMessage(metadata.GetDisplayName()));
        }
    }

    /// <summary>
    /// Specifies the minimum and maximum length of charactors that are allowed in a data field. 
    /// </summary>
    public class UmbracoStringLengthAttribute : StringLengthAttribute, IClientValidatable
    {
        public UmbracoStringLengthAttribute(int maximumLength)
            : base(maximumLength)
        {
            ErrorMessage = UmbracoDictionary.Value("MinMaxLengthError");
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            yield return
                new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), MinimumLength, MaximumLength);
        }

        public UmbracoStringLengthAttribute(int maximumLength, string resourceName)
            : base(maximumLength)
        {
            ErrorMessage = UmbracoDictionary.Value(resourceName);
        }
    }

    

    

    /// <summary>
    /// Specifies the minimum length of array or string data allowed in a property.
    /// </summary>
    public class UmbracoMinLengthAttribute : MinLengthAttribute, IClientValidatable
    {
        public UmbracoMinLengthAttribute(int length)
            : base(length)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ErrorMessage = UmbracoDictionary.Value("MinLengthError");
            yield return
                new ModelClientValidationMinLengthRule(FormatErrorMessage(metadata.GetDisplayName()), Length);
        }

        public UmbracoMinLengthAttribute(int length, string resourceName)
            : base(length)
        {
            ErrorMessage = UmbracoDictionary.Value(resourceName);
        }
    }

    /// <summary>
    /// Specifies that a data field value in ASP.net Dynamic Data must match the specified regular expression.
    /// </summary>
    public class UmbracoRegularExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public new string ErrorMessageString { get; internal set; }
        public string ResourceName { get; set; } = "RegexError";

        public UmbracoRegularExpressionAttribute(string pattern)
            : base(pattern)
        {
            ErrorMessageString = UmbracoDictionary.Value(ResourceName);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRegexRule(FormatErrorMessage(metadata.GetDisplayName()), Pattern);
        }
    }

    /// <summary>
    /// Specifies that a data field value must be a valid Email Address
    /// </summary>
    public class UmbracoEmailAddressAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public string ResourceName { get; set; } = "EmailError";

        private static new string Pattern { get; set; } = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        public UmbracoEmailAddressAttribute()
            : base(Pattern)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ErrorMessage = UmbracoDictionary.Value(ResourceName);
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

        public static string Value(string resourceKey)
        {
            string key = Helper.GetDictionaryValue(resourceKey);
            if (!string.IsNullOrEmpty(key))
                return key;
            return resourceKey; // Fallback with the key name
        }
    }
}