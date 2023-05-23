using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace KingspModel.Attributes
{
    /// <summary>
    /// 驗證email
    /// </summary>
    public class Email : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            if (value.ToMyString().IsNullOrEmpty()) return true;
            return Regex.IsMatch(value.ToMyString(), Function.EMAIL_REGEX);
        }

        //public override string FormatErrorMessage(string name)
        //{
        //    return base.FormatErrorMessage(name);
        //}

        #region IClientValidatable 成員

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new ModelClientValidationRule[] {
                new ModelClientValidationRule {
                    ValidationType = "email",
                    ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
                }
            };
        }

        #endregion
    }
}
