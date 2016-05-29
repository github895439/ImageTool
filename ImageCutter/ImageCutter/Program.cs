using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;

namespace ImageCutter
{
    class Program
    {
        const String MESSAGE_HELP_1 = "<EXE> <カット対象の画像ファイル名> <カット矩形左上のX座標> <カット矩形左上のY座標> <矩形の幅> <矩形の高さ> <出力ファイル名>";
        const String MESSAGE_HELP_2 = "　座標はペイントブラシでファイルを開きカーソルを置いた時のステータスバー表示等。";
        const String MESSAGE_HELP_3 = "　サイズはペイントブラシで矩形選択時のステータスバー表示等。";
        const int END_VALUE_NG = -1;
        const int END_VALUE_OK = 0;

        static int Main(string[] args)
        {
            int ArgStartX;
            int ArgStartY;
            int ArgSizeW;
            int ArgSizeH;
            Bitmap bitmap;
            Bitmap bitmapPart;
            Rectangle rectangle;

            //引数の数が正しいか
            if (args.Length != 6)
            {
                Console.WriteLine(MESSAGE_HELP_1);
                Console.WriteLine(MESSAGE_HELP_2);
                Console.WriteLine(MESSAGE_HELP_3);
                return END_VALUE_NG;
            }

            //引数の数値化
            try
            {
                ArgStartX = Convert.ToInt32(args[1]);
                ArgStartY = Convert.ToInt32(args[2]);
                ArgSizeW = Convert.ToInt32(args[3]);
                ArgSizeH = Convert.ToInt32(args[4]);
            }
            catch (Exception e)
            {
                Console.WriteLine("引数が不正です。");
                Console.WriteLine(e);
                return END_VALUE_NG;
            }

            //対象ファイル読み込み
            try
            {
                bitmap = new Bitmap(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine("対象ファイルを読み込めません。");
                Console.WriteLine(e);
                return END_VALUE_NG;
            }

            rectangle = new Rectangle(ArgStartX, ArgStartY, ArgSizeW, ArgSizeH);

            //矩形抽出
            try
            {
                bitmapPart = bitmap.Clone(rectangle, bitmap.PixelFormat);
            }
            catch (Exception e)
            {
                Console.WriteLine("矩形を抽出できません。");
                Console.WriteLine(e);
                return END_VALUE_NG;
            }

            //矩形出力
            try
            {
                bitmapPart.Save(args[5]);
            }
            catch (Exception e)
            {
                Console.WriteLine("矩形を出力できません。");
                Console.WriteLine(e);
                return END_VALUE_NG;
            }

            return END_VALUE_NG;
        }
    }
}
