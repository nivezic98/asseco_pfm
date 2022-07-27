using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using PersonalFinanceManagement.API.Database.Entities;
using CsvHelper;
using System.Globalization;
using PersonalFinanceManagement.API.Commands;

namespace PersonalFinanceManagement.API.Formatters
{
    public class CategoryInput : TextInputFormatter
    {
        public CategoryInput()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/csv"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanReadType(Type type)
        {
            if (type == typeof(CreateCategoryList))
            {
                return base.CanReadType(type);
            }
            return false;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            var request = context.HttpContext.Request;
            using var reader = new StreamReader(request.Body, encoding);

            try
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    CreateCategoryList categoryList = new CreateCategoryList();

                    await csv.ReadAsync();
                    csv.ReadHeader();

                    while (await csv.ReadAsync())
                    {
                        string code = csv.GetField<string>("code").Trim();
                        string parentCode = csv.GetField<string>("parent-code").Trim();
                        string name = csv.GetField<string>("name").Trim();


                        categoryList.Categories.Add(new CreateCategoryCommand
                        {
                            Code = code,
                            ParentCode = parentCode,
                            Name = name
                        });

                    }

                    return await InputFormatterResult.SuccessAsync(categoryList);

                }
            }
            catch
            {
                return await InputFormatterResult.FailureAsync();
            }
        }
    }
}
