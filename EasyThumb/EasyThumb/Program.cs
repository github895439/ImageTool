using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;

namespace EasyThumb
{
    class Program
    {
        const String MESSAGE_HELP = "<EXE> <構成ファイル名の接尾語(拡張子含む)> <横方向並べ数> <出力ファイル名>";
        const int END_VALUE_NG = -1;
        const int END_VALUE_OK = 0;

        static String folder = String.Empty;
        static Size imageSize;

        static int Main(string[] args)
        {
            int countSide;
            int sizeThumbW = int.MinValue;
            int sizeThumbH = int.MinValue;
            int countFile = int.MinValue;
            Bitmap thumb;

            //引数の数が正しいか
            if (args.Length != 3)
            {
                Console.WriteLine(MESSAGE_HELP);
                return END_VALUE_NG;
            }

            //引数の数値化
            try
            {
                countSide = Convert.ToInt32(args[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine("引数<横方向並べ数>が不正です。");
                Console.WriteLine(e);
                return END_VALUE_NG;
            }

            //サムネのサイズを算出、ファイル数把握
            if (Program.CalcSizeEtc(args[0], countSide, ref sizeThumbW, ref sizeThumbH, ref countFile) != END_VALUE_OK)
            {
                //内容は下位が出力
                return END_VALUE_NG;
            }

            Console.WriteLine("出力されるサイズ:" + sizeThumbW + " x " + sizeThumbH);
            Console.WriteLine("構成ファイル数:" + countFile);
            Console.WriteLine("中止する場合はCTRL+cを押して下さい。");
            Console.ReadLine();

            thumb = new Bitmap(sizeThumbW, sizeThumbH);

            //サムネ作成
            if (Program.Placing(args[0], countFile, countSide, thumb) != END_VALUE_OK)
            {
                //内容は下位が出力
                return END_VALUE_NG;
            }

            //サムネ出力
            try
            {
                thumb.Save(args[2]);
            }
            catch (Exception e)
            {
                Console.WriteLine("サムネを出力できません。");
                Console.WriteLine(e);
                return END_VALUE_NG;
            }

            return END_VALUE_NG;
        }

        static int CalcSizeEtc(String suffix, int countSide, ref int sizeThumbW, ref int sizeThumbH, ref int countFile)
        {
            String filename;
            int count = 0;
            Bitmap imageFirst;

            //接尾語にパスが含まれていた場合の分解
            try
            {
                Program.folder = Path.GetDirectoryName(suffix);

                //パスが含まれているか
                if (Program.folder != String.Empty)
                {
                    Program.folder += @"\";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("引数<構成ファイル名の接尾語(拡張子含む)>が不正です。");
                Console.WriteLine(e);
                return END_VALUE_NG;
            }

            filename = Program.CreateFilename(count, suffix);

            //最初の構成ファイルが存在しないか
            if (!File.Exists(filename))
            {
                Console.WriteLine("構成ファイルがありません。(" + filename + ")");
                return END_VALUE_NG;
            }

            //サイズ把握のために最初の構成ファイル読み込み
            try
            {
                imageFirst = new Bitmap(filename);
            }
            catch (Exception e)
            {
                Console.WriteLine("構成ファイルを読み込めませんでした。(" + filename + ")");
                Console.WriteLine(e);
                return END_VALUE_NG;
            }

            Program.imageSize = imageFirst.Size;
            sizeThumbW = Program.imageSize.Width * countSide;

            //構成ファイル数カウントループ
            do
            {
                count++;
                filename = Program.CreateFilename(count, suffix);
            } while (File.Exists(filename) && count < int.MaxValue);

            //構成ファイルが多すぎるか
            if (count == int.MaxValue)
            {
                Console.WriteLine("構成ファイルが多すぎます。");
                return END_VALUE_NG;
            }

            countFile = count;
            sizeThumbH = countFile % countSide == 0 ? countFile / countSide : countFile / countSide + 1;
            sizeThumbH *= Program.imageSize.Height;
            return END_VALUE_OK;
        }

        static int Placing(String suffix, int countFile, int countSide, Bitmap thumb)
        {
            String filename;
            int placeX;
            int placeY;
            Bitmap part;

            using (Graphics placing = Graphics.FromImage(thumb))
            {
                //配置ループ
                for (int i = 0; i < countFile; i++)
                {
                    filename = Program.CreateFilename(i, suffix);

                    //構成ファイル読み込み
                    try
                    {
                        part = new Bitmap(filename);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("構成ファイルを読み込めませんでした。(" + filename + ")");
                        Console.WriteLine(e);
                        return END_VALUE_NG;
                    }

                    //構成ファイルのサイズチェック
                    if ((part.Size.Width != Program.imageSize.Width) || (part.Size.Height != Program.imageSize.Height))
                    {
                        Console.WriteLine("構成ファイルのサイズが異なります。(" + filename + ")");
                        return END_VALUE_NG;
                    }

                    placeX = (i % countSide) * Program.imageSize.Width;
                    placeY = (i / countSide) * Program.imageSize.Height;
                    placing.DrawImage(part, placeX, placeY);
                }

                placing.Save();
            }

            return END_VALUE_OK;
        }

        static String CreateFilename(int index, String suffix)
        {
            String ret;

            ret = Program.folder + index.ToString() + "_" + Path.GetFileName(suffix);
            return ret;
        }
    }
}
