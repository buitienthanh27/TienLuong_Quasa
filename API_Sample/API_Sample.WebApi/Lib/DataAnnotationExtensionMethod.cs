using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;

namespace API_Sample.WebApi.Lib
{
    public static class DataAnnotationExtensionMethod
    {
        public static string GetErrorMessage(this Controller controllers)
        {
            StringBuilder error = new StringBuilder();
            var errors = controllers.ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            foreach (var errs in errors)
                foreach (var item in errs)
                    error.Append($"{item.ErrorMessage}\r\n");
            return error.ToString();
        }

        public static string GetErrorMessage(ModelStateDictionary modelState)
        {
            StringBuilder error = new StringBuilder();
            var errors = modelState.Where(w => w.Value.ValidationState == ModelValidationState.Invalid).ToList();
            errors.ForEach(item =>
            {
                //error += $"Key: {item.Key}; Value: {item.Value.ValidationState}; Message: {item.Value.Errors.Select(s => s.ErrorMessage).First()}";
                //error += $"{item.Key}: {item.Value.Errors.Select(s => s.ErrorMessage).First()}\r\n";
                error.Append($"{item.Key}: {item.Value.Errors.Select(s => s.ErrorMessage).First()}\r\n");
            });
            return error.ToString();
        }
    }
}
