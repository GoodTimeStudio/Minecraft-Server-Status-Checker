using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI;
using Minecraft_Server_Status_Checker.Status;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;

namespace Minecraft_Server_Status_Checker.Status.Motd
{
    public class TextComponent : TextComponentBase
    {
        public string text;
        public List<TextComponent> extra;

        /// <summary>
        /// 进行一些处理，当此对象为基对象时才调用此方法
        /// </summary>
        internal void SetupBaseComponent()
        {
            TransformTextToComponent();
        }

        private void TransformTextToComponent()
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (extra == null)
                {
                    extra = new List<TextComponent>();
                }
                List<TextComponent> NewExtra = new List<TextComponent>();

                Regex reg = new Regex("§");
                //获取motd中样式代码的个数
                int codeCount = reg.Matches(text).Count;

                ////current
                string subStr;
                string codeStr;
                TextComponent component = new TextComponent(); ;

                int lastIndex = -1;

                if (codeCount == 0)
                {
                    TextComponent sub = new TextComponent();
                    sub.text = text;
                    NewExtra.Add(sub);
                }
                else
                {
                    for (int i = 0; i <= codeCount; i++)
                    {
                        int index = text.IndexOf("§", lastIndex + 1);

                        if (lastIndex >= 0 && index > 0)
                        {
                            //与上一个分隔符的间隔
                            int spacing = index - lastIndex;

                            if (spacing >= 3)
                            {
                                subStr = text.Substring(lastIndex, spacing);
                                codeStr = subStr.Substring(0, 2);
                                component.text = subStr.Substring(2);
                                ParseFormattingCode(codeStr, component);
                                NewExtra.Add(component);

                                component = new TextComponent();
                            }
                            else if (spacing >= 2) // 两个连续的分隔符 §a§a
                            {
                                codeStr = text.Substring(lastIndex, spacing);
                                ParseFormattingCode(codeStr, component);
                            }
                            else //当两个分隔符在一起时 §§
                            {
                                component.Reset();
                                component.color = ColorCode.White;
                                index = text.IndexOf("§", lastIndex + 2);
                            }

                        }
                        else if (lastIndex > index) // 最后一个分隔符
                        {
                            subStr = text.Substring(lastIndex);
                            codeStr = subStr.Substring(0, 2);
                            component.text = subStr.Substring(2);
                            ParseFormattingCode(codeStr, component);
                            NewExtra.Add(component);
                        }

                        lastIndex = index;
                    }
                }

                extra.InsertRange(0, NewExtra);
                text = "";
            }
                
        }

        private void ParseFormattingCode(string code, TextComponent component)
        {

            StyleCode style = StyleCode.GetTypeFromCode(code);
            if (style != null)
            {
                if (style == StyleCode.Obfuscated)
                    component.obfuscated = true;
                else if (style == StyleCode.Bold)
                    component.bold = true;
                else if (style == StyleCode.Strikethrough)
                    component.strikethrough = true;
                else if (style == StyleCode.Underline)
                    component.underlined = true;
                else if (style == StyleCode.Italic)
                    component.italic = true;
                else if (style == StyleCode.Reset)
                    component.Reset();
            }
            else  //如果不是 StyleCode
            {
                ColorCode color = ColorCode.GetColorCodeFromCode(code);
                if (color == null)
                {
                    component.Reset();
                    component.color = ColorCode.White;
                }
                component.color = color;
            }
        }

    }

    public class TextComponentConverter : CustomCreationConverter<TextComponent>
    {
        public override TextComponent Create(Type objectType)
        {
            return new TextComponent();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            TextComponent ret;
            if (reader.ValueType == typeof(string))
            {
                ret = new TextComponent();
                ret.text = reader.Value as string;
            }
            else
            {
                ret = base.ReadJson(reader, objectType, existingValue, serializer) as TextComponent;
            }
            ret.SetupBaseComponent();
            return ret;
        }
    }
}
