using CadeODinheiro.Web.Infrastructure.Helpers.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace CadeODinheiro.Web.Infrastructure.Helpers
{
    public class TBOSHelpers
    {
        private HtmlHelper helper;
        public TBOSHelpers(HtmlHelper helperParam)
        {
            helper = helperParam;
        }

        public IHtmlString TextBox(string id, TextBoxType type = TextBoxType.Texto, bool exibirMsgValidacao = true, object htmlAttributes = null)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var labelBuilder = new TagBuilder("label");
            labelBuilder.AddCssClass("input");

            switch (type)
            {
                case TextBoxType.Nome:
                    var iconBuilderNome = new TagBuilder("i");
                    iconBuilderNome.MergeAttribute("class", "icon-prepend fa fa-user");
                    labelBuilder.InnerHtml += iconBuilderNome;
                    break;
                case TextBoxType.Email:
                    var iconBuilderEmail = new TagBuilder("i");
                    iconBuilderEmail.MergeAttribute("class", "icon-prepend fa fa-envelope-o");
                    labelBuilder.InnerHtml += iconBuilderEmail;
                    break;
                case TextBoxType.Telefone:
                    var iconBuilderTelefone = new TagBuilder("i");
                    iconBuilderTelefone.MergeAttribute("class", "icon-prepend fa fa-phone");
                    labelBuilder.InnerHtml += iconBuilderTelefone;
                    break;
                case TextBoxType.Cep:
                    attributes.Add("data-mask", "99.999-999");
                    break;
                case TextBoxType.Senha:
                    var iconBuilderSenha = new TagBuilder("i");
                    iconBuilderSenha.MergeAttribute("class", "icon-prepend fa fa-lock");
                    labelBuilder.InnerHtml += iconBuilderSenha;
                    break;
            }

            var textBoxHelper = helper.TextBox(id, helper.Value(id).ToHtmlString(), attributes);
            labelBuilder.InnerHtml += textBoxHelper;


            var tagMsgValidation = string.Empty;

            if (!helper.ViewData.ModelState.IsValidField(id))
            {
                labelBuilder.AddCssClass("state-error");

                if (exibirMsgValidacao)
                {
                    var validationBuilder = helper.ValidationMessage(id).ToString().Replace("span", "em");
                    tagMsgValidation += validationBuilder.ToString();
                }

            }

            var htmlRetorno = labelBuilder + tagMsgValidation;

            return MvcHtmlString.Create(htmlRetorno);
        }
    }
}