  using System.Text.RegularExpressions;
  using Umbraco.Core;
  
  namespace Umbraco.Web.Templates
  {
      public static class TemplateUtillitiesExtensions
      {
          public static string ParseInternalMediaLinks(string value ) {
  
              if(UmbracoContext.Current == null) {
                  return value;
              }
  
              var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
  
              //get all anchors and images with data-id, href's including media and get their values
              string pattern = @"\<(?:a|img)(?=.*?data-id\=\""([0-9]+))(?=.*?(?:(?:src|href)\=\"")(\/media\/\S+)\"")";
              foreach(Match match in Regex.Matches(value, pattern, RegexOptions.Multiline))
              {
                  Group dataIdGrp = match.Groups[1];
                  Group urlGrp = match.Groups[2];
  
                  var image = umbracoHelper.TypedMedia(dataIdGrp.Value);
                  if(image == null) continue;
  
                  if(urlGrp.Value == image.Url) continue;
  
                  value = value.Replace(urlGrp.Value, image.Url);
              }
  
              return value;
          }
      }
  }