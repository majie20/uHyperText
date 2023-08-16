using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace WXB
{
    public class SymbolTextInit : MonoBehaviour
    {
        static Dictionary<string, Font>    Fonts; // 当前所有的字库
        static Dictionary<string, DSprite> Sprites; // 当前所有的精灵
        static Dictionary<string, Cartoon> Cartoons; // 当前所有的动画

        [SerializeField]
        Font[] fonts = null;

        [SerializeField]
        Sprite[] sprites = null;

        [SerializeField]
        Cartoon[] cartoons = null; // 所有的动画

        void init()
        {
            if (Fonts == null)
                Fonts = new Dictionary<string, Font>();
            else
                Fonts.Clear();

            if (fonts != null)
            {
                for (int i = 0; i < fonts.Length; ++i)
                    Fonts.Add(fonts[i].name, fonts[i]);
            }

            if (Sprites == null)
                Sprites = new Dictionary<string, DSprite>();
            else
                Sprites.Clear();

            if (sprites != null)
            {
                for (int i = 0; i < sprites.Length; ++i)
                {
                    Sprites.Add(sprites[i].name, new DSprite(sprites[i], sprites[i].name));
                }
            }

            if (Cartoons == null)
                Cartoons = new Dictionary<string, Cartoon>();
            else
                Cartoons.Clear();

            if (cartoons != null)
            {
                for (int i = 0; i < cartoons.Length; ++i)
                {
                    if (i == 0)
                    {
                        var frames = new Cartoon.Frame[5];

                        for (int j = 0; j < frames.Length; j++)
                        {
                            frames[j] = new Cartoon.Frame() { delay = 0.1f, sprite = new DSprite(Resources.Load<Sprite>($"anim_{cartoons[i].name}_{j + 1}")) };
                        }

                        cartoons[i].frames = frames;
                    }
                    else if (i == 1)
                    {
                        OCartoon oCartoon = new OCartoon();
                        oCartoon.col = 9;
                        oCartoon.row = 1;
                        oCartoon.width = 30;
                        oCartoon.height = 30;
                        oCartoon.name = "i500";
                        oCartoon.space = 2;
                        oCartoon.frame = new Cartoon.Frame() { delay = 0.1f, sprite = new DSprite(Resources.Load<Sprite>($"{oCartoon.name}")) };
                        cartoons[i] = oCartoon;
                    }
                    else if (i == 2)
                    {
                        OCartoon oCartoon = new OCartoon();
                        oCartoon.col = 5;
                        oCartoon.row = 2;
                        oCartoon.width = 30;
                        oCartoon.height = 30;
                        oCartoon.name = "i501";
                        oCartoon.space = 2;
                        oCartoon.frame = new Cartoon.Frame() { delay = 0.1f, sprite = new DSprite(Resources.Load<Sprite>($"{oCartoon.name}")) };
                        cartoons[i] = oCartoon;
                    }

                    Cartoons.Add(cartoons[i].name, cartoons[i]);
                }
            }

            //GetComponent<RawImage>().uvRect
        }

        static void Init()
        {
            Resources.Load<SymbolTextInit>("SymbolTextInit").init();
        }

        public static Font GetFont(string name)
        {
            if (Fonts == null)
                Init();

            Font font;

            if (Fonts.TryGetValue(name, out font))
                return font;

            return null;
        }

        public static ISprite GetSprite(string name)
        {
            if (Sprites == null)
                Init();

            DSprite sprite;

            if (Sprites.TryGetValue(name, out sprite))
                return sprite;

            return null;
        }

        public static Cartoon GetCartoon(string name)
        {
            if (Cartoons == null)
                Init();

            Cartoon cartoon;

            if (Cartoons.TryGetValue(name, out cartoon))
                return cartoon;

            return null;
        }

        public static void GetCartoons(List<Cartoon> cartoons)
        {
            if (Cartoons == null)
                Init();

            foreach (var itor in Cartoons)
            {
                cartoons.Add(itor.Value);
            }
        }
    }
}