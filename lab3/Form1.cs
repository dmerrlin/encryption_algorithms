using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab3
{
    public partial class Form1 : Form
    {
        //Шифр простой замены
         string rus_alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        char encryption(char ch)
        {
            int ret = 34;
            for (int i = 0; i < rus_alphabet.Length; i++)
            {
                if (textBox3.Text[i] == ch) ret = i;
            }
            if (ret != 34)
                return rus_alphabet[ret];
            else return ch;
        }
        ///////////////////////////////////////////////////////////////////

        //RSA
        ulong mod1(byte a, ulong n, int e)

        {

            ulong return_mod = 1;

            for (int i = 0; i < e; i++)

            {

                return_mod *= a;

                return_mod = return_mod % n;

            }

            return return_mod % n;

        }

        byte mod2(ulong a, ulong n, int d)

        {

            ulong return_mod = 1;

            for (int i = 0; i < d; i++)

            {

                return_mod *= a;

                return_mod = return_mod % n;

            }

            return (byte)(return_mod % n);
        }
        ///////////////////////////////////////////////////////////////////

        //MD4
        uint F(uint x, uint y, uint z)

        {

            uint xy = x & y;

            uint _xz = z & ~x;

            return xy | _xz;

        }

        uint G(uint x, uint y, uint z)

        {

            uint xy = x & y;

            uint xz = x & z;

            uint yz = y & z;

            return xy | xz | yz;

        }

        uint H(uint x, uint y, uint z)

        {

            return x ^ y ^ z;

        }

        uint main_func(uint A, uint B, uint C, uint D, int k, int s, int type_func)

        {

            if (type_func == 0)

            {

                uint result = A + F(B, C, D) + X[k];

                result = (uint)((result << s) | (result >> (32 - s)));

                return result;

            }

            if (type_func == 1)

            {

                uint result = A + G(B, C, D) + X[k] + 0x5A827999;

                result = (uint)((result << s) | (result >> (32 - s)));

                return result;

            }

            if (type_func == 2)

            {

                uint result = A + H(B, C, D) + X[k] + 0x6ED9EBA1;

                result = (uint)((result << s) | (result >> (32 - s)));

                return result;

            }

            return 0;

        }
        ///////////////////////////////////////////////////////////////////
        public bool IsCoprime(int num1, int num2)
        {
            if (num1 == num2)
            {
                return num1 == 1;
            }
            else
            {
                if (num1 > num2)
                {
                    return IsCoprime(num1 - num2, num2);
                }
                else
                {
                    return IsCoprime(num2 - num1, num1);
                }
            }
        }

        public static bool IsPrimeNumber(int n)
        {
            var result = true;

            if (n > 1)
            {
                for (var i = 2u; i < n; i++)
                {
                    if (n % i == 0)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
        // ГОСТ
        string encode_gost(string text_e, string key, int rounds)

        {

            byte[] byteText = Encoding.Default.GetBytes(text_e); //перевод в байты текста

            byte[] byteKey = Encoding.Default.GetBytes(key); //перевод в байты ключа

            uint[] uintKey = GetUIntKeyArray(byteKey);

            ulong[] ulongText = GetULongDataArray(byteText);

            ulong[] ulongEncrText = new ulong[ulongText.Length];

            for (int k = 0; k < ulongText.Length; k++)

            {

                ulongEncrText[k] = ulongText[k];

              //  for (int j = 0; j < 3; j++)

                    for (int i = 0; i < 4; i++)

                    {

                        ulongEncrText[k] = baseStep(ulongEncrText[k], uintKey[i], false);

                    }

                for (int i = 3; i >= 0; i--)

                {

                    if (i != 0)

                        ulongEncrText[k] = baseStep(ulongEncrText[k], uintKey[i], false);

                    else

                        ulongEncrText[k] = baseStep(ulongEncrText[k], uintKey[i], true);

                }

            }

            return Encoding.Default.GetString(ConvertToByte(ulongEncrText));

        }

        string decode_gost(string text_e, string key, int rounds)

        {

            byte[] byteText = Encoding.Default.GetBytes(text_e); //перевод в байты текста

            byte[] byteKey = Encoding.Default.GetBytes(key); //перевод в байты ключа

            uint[] uintKey = GetUIntKeyArray(byteKey);

            ulong[] ulongText = GetULongDataArray(byteText);

            ulong[] ulongEncrText = new ulong[ulongText.Length];

            for (int k = 0; k < ulongText.Length; k++)

            {

                ulongEncrText[k] = ulongText[k];

                for (int i = 0; i < 4; i++)

                {

                    ulongEncrText[k] = baseStep(ulongEncrText[k], uintKey[i], false);

                }

               // for (int j = 0; j < 3; j++)

                    for (int i = 3; i >= 0; i--)

                    {

                        if ((i == 0))

                            ulongEncrText[k] = baseStep(ulongEncrText[k], uintKey[i], true);

                        else

                            ulongEncrText[k] = baseStep(ulongEncrText[k], uintKey[i], false);

                    }

            }

            return Encoding.Default.GetString(ConvertToByte(ulongEncrText));

        }

        public ulong[] GetULongDataArray(byte[] byteData)

        {

            ulong[] data = new ulong[byteData.Length / 8];

            for (int i = 0; i < data.Length; i++)

            {

                data[i] = BitConverter.ToUInt64(byteData, i * 8);

            }

            return data;

        }

        public byte[] ConvertToByte(ulong[] fl)

        {

            byte[] temp = new byte[8];

            byte[] encrByteFile = new byte[fl.Length * 8];

            for (int i = 0; i < fl.Length; i++)

            {

                temp = BitConverter.GetBytes(fl[i]);

                for (int j = 0; j < temp.Length; j++)

                    encrByteFile[j + i * 8] = temp[j];

            }

            return encrByteFile;

        }

        public uint[] GetUIntKeyArray(byte[] byteKey)

        {

            uint[] key = new uint[8];

            for (int i = 0; i < key.Length; i++)

            {

                key[i] = BitConverter.ToUInt32(byteKey, i * 4);

            }

            return key;

        }

        public ulong baseStep(ulong dateFragment, uint keyFragment, bool last)

        {

            uint N1, N2, X;

            N2 = (uint)(dateFragment >> 32);

            N1 = (uint)((dateFragment << 32) >> 32);

            X = keyFragment;

            uint S = (uint)((X + N1) % (Convert.ToUInt64(Math.Pow(2, 32)))); //первый шаг

            S = Second_ThirdStep(S); //второй и третий шаг основного шага

            S = (S ^ N2); //четвертый шаг

            if (last == false)

            {

                N2 = N1; //пятый шаг

                N1 = S;

            }

            else

            {

                N2 = S;

            }

            //шестой шаг

            return ((ulong)N2) << 32 | (((ulong)N1));

        }

        private uint Second_ThirdStep(uint S)

        {

            uint newS, S0, S1, S2, S3, S4, S5, S6, S7;

            S0 = S >> 28;

            S1 = (S << 4) >> 28;

            S2 = (S << 8) >> 28;

            S3 = (S << 12) >> 28;

            S4 = (S << 16) >> 28;

            S5 = (S << 20) >> 28;

            S6 = (S << 24) >> 28;

            S7 = (S << 28) >> 28;

            byte[] Szam = {

                        0xA,0x4,0x5,0x6,0x8,0x1,0x3,0x7,0xD,0xC,0xE,0x0,0x9,0x2,0xB,0xF,
                        0x5,0xF,0x4,0x0,0x2,0xD,0xB,0x9,0x1,0x7,0x6,0x3,0xC,0xE,0xA,0x8,
                        0x7,0xF,0xC,0xE,0x9,0x4,0x1,0x0,0x3,0xB,0x5,0x2,0x6,0xA,0x8,0xD,
                        0x4,0xA,0x7,0xC,0x0,0xF,0x2,0x8,0xE,0x1,0x6,0x5,0xD,0xB,0x9,0x3,
                        0x7,0x6,0x4,0xB,0x9,0xC,0x2,0xA,0x1,0x8,0x0,0xE,0xF,0xD,0x3,0x5,
                        0x7,0x6,0x2,0x4,0xD,0x9,0xF,0x0,0xA,0x1,0x5,0xB,0x8,0xE,0xC,0x3,
                        0xD,0xE,0x4,0x1,0x7,0x0,0x5,0xA,0x3,0xC,0x8,0xF,0x6,0x2,0x9,0xB,
                        0x1,0x3,0xA,0x9,0x5,0xB,0x4,0xF,0x8,0x6,0x7,0xE,0xD,0x0,0x2,0xC

                        };

            S0 = Szam[S0];

            S1 = Szam[0x10 + S1];

            S2 = Szam[0x20 + S2];

            S3 = Szam[0x30 + S3];

            S4 = Szam[0x40 + S4];

            S5 = Szam[0x50 + S5];

            S6 = Szam[0x60 + S6];

            S7 = Szam[0x70 + S7];

            newS = S7 + (S6 << 4) + (S5 << 8) + (S4 << 12) + (S3 << 16) +

            (S2 << 20) + (S1 << 24) + (S0 << 28);

            return (uint)(newS << 11) | (newS >> 21);

        }

        /////////////////////////////////////////////////////////


        public Form1()
        {
            InitializeComponent();
        }

        private void bindingNavigatorMoveNextItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";


                for (int j = 0; j < textBox1.Text.Length; j++)
                {
                textBox2.Text += encryption(textBox1.Text[j]);
                }

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";

           int p = 1229;

            int q = 1181;

            int n = q * p;

            // int ee = 322009;
            int ee = 2;
            Random rnd = new Random();
            for (int i = 3; i < n; i++)
            {
                if (IsPrimeNumber(i))
                {
                    if (n % i != 0)
                    {
                        ee = i;
                        break;

                    }
                }
            }

            int d = 3;

                do
                {
                    d++;

                } while (((d * ee) % ((p - 1) * (q - 1))) != 1);


            byte[] byte_string = new byte[textBox4.Text.Length];

            ulong[] encode = new ulong[textBox4.Text.Length];

            byte[] decode = new byte[textBox4.Text.Length];

            byte_string = Encoding.Default.GetBytes(textBox4.Text);

            richTextBox1.Text += "p = "+p+ "\n";
            richTextBox1.Text += "q = " + q + "\n";
            richTextBox1.Text += "n = " + n + "\n";
            richTextBox1.Text += "e = " + ee + "\n";
            richTextBox1.Text += "d = " + d + "\n";

            richTextBox1.Text += "Cообщение: " + textBox4.Text + " в байтах: ";

           

            for (int i = 0; i < textBox4.Text.Length; i++)

            {

                richTextBox1.Text += byte_string[i].ToString() + " ";

            }

            richTextBox1.Text += "\nЗашифрованное сообщение: ";

            for (int i = 0; i < textBox4.Text.Length; i++) //шифрую открытым ключем e,n

            {

                encode[i] = mod1(byte_string[i], (ulong)n, ee);

                richTextBox1.Text += encode[i].ToString() + " ";

            }

            richTextBox1.Text += "\n";

            richTextBox1.Text += "Расшифрованное сообщение: ";

            for (int i = 0; i < textBox4.TextLength; i++) //шифрую секретным ключем d,n

            {

                decode[i] = mod2(encode[i], (ulong)n, d);

                richTextBox1.Text += decode[i].ToString() + " ";

            }

            richTextBox1.Text += "--> " + Encoding.Default.GetString(decode);

        }

       uint[] X = new uint[16];
        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "Алгоритм MD4\n";

            string str = textBox5.Text;

            //добавление дополительных битов.

            ushort need_symbols = (ushort)((512 - ((str.Length * 8 + 64) % 512)) / 8);

            richTextBox2.Text += "Нужно добавить:" + need_symbols.ToString() + "\n";

            byte[] str_byte = new byte[str.Length + need_symbols + 8];

            byte[] str_byte_2 = Encoding.Default.GetBytes(str); //доп биты

            for (int i = 0; i < str_byte_2.Length; i++)

                str_byte[i] = str_byte_2[i];

            str_byte[str.Length] = Convert.ToByte("10000000", 2);

            for (int i = str.Length + 1; i < str_byte.Length; i++)

                str_byte[i] = Convert.ToByte("00000000", 2);

            //конец добавление доп битов

            //добавление 64-бит слова

            ulong size_messedge = (ulong)str.Length * 8;

            byte[] str_byte_3 = BitConverter.GetBytes(size_messedge);

            //конец добвавления 64 битного слова.

            str_byte[str.Length + need_symbols + 0] = str_byte_3[0]; //если вдруг перестанет работать, то ковырять тут

            str_byte[str.Length + need_symbols + 1] = str_byte_3[1];

            str_byte[str.Length + need_symbols + 2] = str_byte_3[2];

            str_byte[str.Length + need_symbols + 3] = str_byte_3[3];

            str_byte[str.Length + need_symbols + 4] = str_byte_3[4];

            str_byte[str.Length + need_symbols + 5] = str_byte_3[5];

            str_byte[str.Length + need_symbols + 6] = str_byte_3[6];

            str_byte[str.Length + need_symbols + 7] = str_byte_3[7];

            richTextBox2.Text += "Количество байт в итоге: " + str_byte.Length + "\n";

            uint A = 0x67452301;

            uint B = 0xefcdab89;

            uint C = 0x98badcfe;

            uint D = 0x10325476;

            for (int i = 0; i < str_byte.Length / 64; i++)

            {

                uint AA = A;

                uint BB = B;

                uint CC = C;

                uint DD = D;

                for (int j = 0; j < 16; j++)

                {

                    X[j] = BitConverter.ToUInt32(str_byte, i * 64 + j * 4);

                }

                // ПЕРВЫЙ РАУНД

                A = main_func(A, B, C, D, 0, 3, 0);

                D = main_func(D, A, B, C, 1, 7, 0);

                C = main_func(C, D, A, B, 2, 11, 0);

                B = main_func(B, C, D, A, 3, 19, 0);

                A = main_func(A, B, C, D, 4, 3, 0);

                D = main_func(D, A, B, C, 5, 7, 0);

                C = main_func(C, D, A, B, 6, 11, 0);

                B = main_func(B, C, D, A, 7, 19, 0);

                A = main_func(A, B, C, D, 8, 3, 0);

                D = main_func(D, A, B, C, 9, 7, 0);

                C = main_func(C, D, A, B, 10, 11, 0);

                B = main_func(B, C, D, A, 11, 19, 0);

                A = main_func(A, B, C, D, 12, 3, 0);

                D = main_func(D, A, B, C, 13, 7, 0);

                C = main_func(C, D, A, B, 14, 11, 0);

                B = main_func(B, C, D, A, 15, 19, 0);

                //ВТОРОЙ РАУНД

                A = main_func(A, B, C, D, 0, 3, 1);

                D = main_func(D, A, B, C, 4, 5, 1);

                C = main_func(C, D, A, B, 8, 9, 1);

                B = main_func(B, C, D, A, 12, 13, 1);

                A = main_func(A, B, C, D, 1, 3, 1);

                D = main_func(D, A, B, C, 5, 5, 1);

                C = main_func(C, D, A, B, 9, 9, 1);

                B = main_func(B, C, D, A, 13, 13, 1);

                A = main_func(A, B, C, D, 2, 3, 1);

                D = main_func(D, A, B, C, 6, 5, 1);

                C = main_func(C, D, A, B, 10, 9, 1);

                B = main_func(B, C, D, A, 14, 13, 1);

                A = main_func(A, B, C, D, 3, 3, 1);

                D = main_func(D, A, B, C, 7, 5, 1);

                C = main_func(C, D, A, B, 11, 9, 1);

                B = main_func(B, C, D, A, 15, 13, 1);

                //ТРЕТИЙ РАУНД

                A = main_func(A, B, C, D, 0, 3, 2);

                D = main_func(D, A, B, C, 8, 9, 2);

                C = main_func(C, D, A, B, 4, 11, 2);

                B = main_func(B, C, D, A, 12, 15, 2);

                A = main_func(A, B, C, D, 2, 3, 2);

                D = main_func(D, A, B, C, 10, 9, 2);

                C = main_func(C, D, A, B, 6, 11, 2);

                B = main_func(B, C, D, A, 14, 15, 2);

                A = main_func(A, B, C, D, 1, 3, 2);

                D = main_func(D, A, B, C, 9, 9, 2);

                C = main_func(C, D, A, B, 5, 11, 2);

                B = main_func(B, C, D, A, 13, 15, 2);

                A = main_func(A, B, C, D, 3, 3, 2);

                D = main_func(D, A, B, C, 11, 9, 2);

                C = main_func(C, D, A, B, 7, 11, 2);

                B = main_func(B, C, D, A, 15, 15, 2);

                A = A + AA;

                B = B + BB;

                C = C + CC;

                D = D + DD;

            }

            byte[] A_byte = BitConverter.GetBytes(A);

            byte[] B_byte = BitConverter.GetBytes(B);

            byte[] C_byte = BitConverter.GetBytes(C);

            byte[] D_byte = BitConverter.GetBytes(D);

            for (int i = 0; i < A_byte.Length; i++)

            {

                richTextBox2.Text += Convert.ToString(A_byte[i], 16);

            }

            for (int i = 0; i < A_byte.Length; i++)

            {

                richTextBox2.Text += Convert.ToString(B_byte[i], 16);

            }

            for (int i = 0; i < A_byte.Length; i++)

            {

                richTextBox2.Text += Convert.ToString(C_byte[i], 16);

            }

            for (int i = 0; i < A_byte.Length; i++)

            {

                richTextBox2.Text += Convert.ToString(D_byte[i], 16);

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string message = textBox6.Text;
            richTextBox3.Text = "Сообщение: " + message + "\n";
            richTextBox3.Text += "Ключ: " + textBox3.Text + "\n";
            richTextBox3.Text += "Зашифрованный текс: " + encode_gost(message, textBox3.Text,4) + "\n";
            richTextBox3.Text += "Зашифрованный текс: " + decode_gost(encode_gost(message, textBox3.Text, 4), textBox3.Text, 4);
        }
        ulong mod3(ulong a, ulong n, int d)

        {

            ulong return_mod = 1;

            for (int i = 0; i < d; i++)

            {

                return_mod *= a;

                return_mod = (ulong)(return_mod % n);

            }

            return (ulong)(return_mod % n);

        }
        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox4.Text = "Электронная цифровая подпись по схеме RSA\n";

            //ищу хэш-образ используя функцию Hi= (Hi-1 + Mi)^2 mod n

            int p = 1229;

            int q = 1181;

            ulong n = (ulong)(q * p);

            int ee = 322009;

            int d = 9;


            string hash = "36ffbb2ef7fed6ebc3f113b11f7fb4f4";

            
            byte[] byte_string = new byte[hash.Length];

            ulong[] encode = new ulong[hash.Length];

            byte[] decode = new byte[hash.Length];

            byte_string = Encoding.Default.GetBytes(hash);

            richTextBox4.Text += "Хэш-образ: Орлов : " ;
            for (int i = 0; i < hash.Length; i++)
            {
                
                richTextBox4.Text += byte_string[i] +" ";
            }

            richTextBox4.Text += "\n\n" + "Подпись полученная с помощью закрытого ключа {e, n}: \n";
            for (int i = 0; i < hash.Length; i++)
            {
                encode[i] = mod1(byte_string[i], (ulong)n, ee);
                string r = Convert.ToString(encode[i]);
                richTextBox4.Text += r + " ";
            }
            richTextBox4.Text += "\n\n" + "Проверка подписи с помощью открытого ключа {d, n}: ";
            byte[] checkHash = new byte[hash.Length];

            for (int i = 0; i < checkHash.Length; i++)
            {
                decode[i] = mod2(encode[i], (ulong)n, d);

                richTextBox4.Text += decode[i].ToString() + " ";

            }
        }
    }
}
