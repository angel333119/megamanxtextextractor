using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MMXTE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void extrairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Extrator Megaman X8

            int magic;
            int totaldetextos;
            int ponteiro;
            ushort comparador = 0;
            int verificador;
            string todosOsTextos = "";
            int nomes = 0x10;
            int game = ' '; //PS2 e PC = 0x12 - NSW = 0x1E
            int ppgame = ' '; //proximo ponteiro do jogo PS2 e PC = 0x16 - NSW = 0x22

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Megaman X8 PC/PS2/NSW|*.MCB;*.0589CBA3|Todos os arquivos (*.*)|*.*";
            openFileDialog1.Title = "Abrir arquivo de Megaman X8...";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int numerodearquivosabertos = openFileDialog1.FileNames.Length;

                foreach (String file in openFileDialog1.FileNames)
                {
                    using (BinaryReader br = new BinaryReader(File.OpenRead(file)))

                    {
                        magic = br.ReadInt32(); // Read magic

                        if (magic == 0x2042434D) // Magic PS2
                        {
                            game = 0x12;
                            ppgame = 0x16;
                        }
                        else if (magic == 0x42434D4F) //Magic Nintendo Switch
                        {
                            game = 0x1E;
                            ppgame = 0x22;
                        }
                        else
                        {
                            MessageBox.Show("O arquivo não é um arquivo de Megaman X8 válido!", "AVISO!");
                            return;
                        }

                        br.BaseStream.Seek(game, SeekOrigin.Begin);
                        totaldetextos = br.ReadInt16(); // Lê o valor do total de textos
                        ponteiro = br.ReadInt16(); //Lê o valor do primeiro ponteiro

                        //Lê o primeiro texto pra ver se é nome ou texto
                        br.BaseStream.Seek(game + 2 + totaldetextos * 2 + ponteiro, SeekOrigin.Begin);
                        verificador = br.ReadUInt16();

                        for (int i = 0; i < totaldetextos; i++)
                        {
                            string convertido = ""; //Inicia a variavel convertido, que fará a conversão dos hex em textos

                            if (verificador <= 0x00FF || verificador == 0xFFFD || verificador == 0xFFFF) //Se for algum desses valores, não tem como ser arquivo com nome
                            {
                                br.BaseStream.Seek(game + 2 + totaldetextos * 2 + ponteiro, SeekOrigin.Begin); //O arquivo não tem nomes ASCII
                            }
                            else
                            {
                                br.BaseStream.Seek(game + 2 + totaldetextos * 2 + ponteiro + nomes * totaldetextos + 0x2A, SeekOrigin.Begin); //O arquivo tem nomes ASCII
                            }

                            //Inicia a variavel que vai verificar se o texto acabou ou não
                            bool acabouotexto = false;

                            //Equanto não acabar o texto ele vai repetindo
                            while (acabouotexto == false)
                            {
                                //Lê um byte do texto
                                comparador = br.ReadUInt16();

                                //compara se o byte é a endstring
                                //se for, o programa cria uma nova linha
                                //se não continua pra proxima letra
                                if (comparador == 0xFFFF)
                                {
                                    //Quando chegar em uma endstring ele retorna como o texto tendo acabado (acabouotexto = verdadeiro)
                                    acabouotexto = true;

                                    todosOsTextos += "<end>\r\n";

                                    //Volta pra ler o próximo ponteiro
                                    br.BaseStream.Seek(ppgame + i * 2, SeekOrigin.Begin);

                                    //Lê o ponteiro
                                    ponteiro = br.ReadInt16();
                                }

                                else
                                {
                                    acabouotexto = false;

                                    //Começa a conversão dos caracteres
                                    if (comparador >= 0x1A && comparador <= 0x23)
                                    {
                                        //Convertendo dentro do intervalo de 0 e 9.
                                        convertido = ((char)('0' + (comparador - 0x1A))).ToString();
                                    }
                                    else if (comparador >= 0x24 && comparador <= 0x3D)
                                    {
                                        //Convertendo dentro do intervalo de A e Z.
                                        convertido = ((char)('A' + (comparador - 0x24))).ToString();
                                    }
                                    else if (comparador >= 0x3E && comparador <= 0x57)
                                    {
                                        //Convertendo dentro do intervalo de a e z.
                                        convertido = ((char)('a' + (comparador - 0x3E))).ToString();
                                    }
                                    else if (comparador == 0x00)
                                    {
                                        convertido = ' '.ToString();
                                    }
                                    else if (comparador == 0x01)
                                    {
                                        convertido = ("!");
                                    }
                                    else if (comparador == 0x02)
                                    {
                                        convertido = '"'.ToString();
                                    }
                                    else if (comparador == 0x04)
                                    {
                                        convertido = ("&");
                                    }
                                    else if (comparador == 0x05)
                                    {
                                        convertido = ("(");
                                    }
                                    else if (comparador == 0x06)
                                    {
                                        convertido = (")");
                                    }
                                    else if (comparador == 0x08)
                                    {
                                        convertido = ("+");
                                    }
                                    else if (comparador == 0x09)
                                    {
                                        convertido = ("-");
                                    }
                                    else if (comparador == 0x0A)
                                    {
                                        convertido = (",");
                                    }
                                    else if (comparador == 0x0B)
                                    {
                                        convertido = (".");
                                    }
                                    else if (comparador == 0x10)
                                    {
                                        convertido = ("?");
                                    }
                                    else if (comparador == 0x58)
                                    {
                                        convertido = ("'");
                                    }
                                    else if (comparador == 0x5A)
                                    {
                                        convertido = ("<SINAL_SIGMA>");
                                    }
                                    else if (comparador == 0x9C)
                                    {
                                        convertido = ("À");
                                    }
                                    else if (comparador == 0x9D)
                                    {
                                        convertido = ("Á");
                                    }
                                    else if (comparador == 0x9E)
                                    {
                                        convertido = ("Â");
                                    }
                                    else if (comparador == 0x9F)
                                    {
                                        convertido = ("Ã");
                                    }
                                    else if (comparador == 0xA0)
                                    {
                                        convertido = ("Ä");
                                    }
                                    else if (comparador == 0xA3)
                                    {
                                        convertido = ("Ç");
                                    }
                                    else if (comparador == 0xA4)
                                    {
                                        convertido = ("È");
                                    }
                                    else if (comparador == 0xA5)
                                    {
                                        convertido = ("É");
                                    }
                                    else if (comparador == 0xA6)
                                    {
                                        convertido = ("Ê");
                                    }
                                    else if (comparador == 0xA7)
                                    {
                                        convertido = ("Ë");
                                    }
                                    else if (comparador == 0xA8)
                                    {
                                        convertido = ("Ì");
                                    }
                                    else if (comparador == 0xA9)
                                    {
                                        convertido = ("Í");
                                    }
                                    else if (comparador == 0xAA)
                                    {
                                        convertido = ("Î");
                                    }
                                    else if (comparador == 0xAB)
                                    {
                                        convertido = ("Ï");
                                    }
                                    else if (comparador == 0xAD)
                                    {
                                        convertido = ("Ñ");
                                    }
                                    else if (comparador == 0xAE)
                                    {
                                        convertido = ("Ò");
                                    }
                                    else if (comparador == 0xAF)
                                    {
                                        convertido = ("Ó");
                                    }
                                    else if (comparador == 0xB0)
                                    {
                                        convertido = ("Ô");
                                    }
                                    else if (comparador == 0xB1)
                                    {
                                        convertido = ("Õ");
                                    }
                                    else if (comparador == 0xB2)
                                    {
                                        convertido = ("Ö");
                                    }
                                    else if (comparador == 0xB4)
                                    {
                                        convertido = ("Ù");
                                    }
                                    else if (comparador == 0xB5)
                                    {
                                        convertido = ("Ú");
                                    }
                                    else if (comparador == 0xB6)
                                    {
                                        convertido = ("Û");
                                    }
                                    else if (comparador == 0xB7)
                                    {
                                        convertido = ("Ü");
                                    }
                                    else if (comparador == 0xB8)
                                    {
                                        convertido = ("Ý");
                                    }
                                    else if (comparador == 0xBA)
                                    {
                                        convertido = ("à");
                                    }
                                    else if (comparador == 0xBB)
                                    {
                                        convertido = ("á");
                                    }
                                    else if (comparador == 0xBC)
                                    {
                                        convertido = ("â");
                                    }
                                    else if (comparador == 0xBD)
                                    {
                                        convertido = ("ã");
                                    }
                                    else if (comparador == 0xBE)
                                    {
                                        convertido = ("ä");
                                    }
                                    else if (comparador == 0xC1)
                                    {
                                        convertido = ("ç");
                                    }
                                    else if (comparador == 0xC2)
                                    {
                                        convertido = ("è");
                                    }
                                    else if (comparador == 0xC3)
                                    {
                                        convertido = ("é");
                                    }
                                    else if (comparador == 0xC4)
                                    {
                                        convertido = ("ê");
                                    }
                                    else if (comparador == 0xC5)
                                    {
                                        convertido = ("ë");
                                    }
                                    else if (comparador == 0xC6)
                                    {
                                        convertido = ("ì");
                                    }
                                    else if (comparador == 0xC7)
                                    {
                                        convertido = ("í");
                                    }
                                    else if (comparador == 0xC8)
                                    {
                                        convertido = ("î");
                                    }
                                    else if (comparador == 0xC9)
                                    {
                                        convertido = ("ï");
                                    }
                                    else if (comparador == 0xCB)
                                    {
                                        convertido = ("ò");
                                    }
                                    else if (comparador == 0xCC)
                                    {
                                        convertido = ("ó");
                                    }
                                    else if (comparador == 0xCD)
                                    {
                                        convertido = ("õ");
                                    }
                                    else if (comparador == 0xCE)
                                    {
                                        convertido = ("ô");
                                    }
                                    else if (comparador == 0xCF)
                                    {
                                        convertido = ("ö");
                                    }
                                    else if (comparador == 0xD3)
                                    {
                                        convertido = ("ù");
                                    }
                                    else if (comparador == 0xD4)
                                    {
                                        convertido = ("ú");
                                    }
                                    else if (comparador == 0xD5)
                                    {
                                        convertido = ("û");
                                    }
                                    else if (comparador == 0xD6)
                                    {
                                        convertido = ("ü");
                                    }
                                    else if (comparador == 0xFFFD)
                                    {
                                        convertido = ("\r\n");
                                    }
                                    else
                                    {
                                        //Os valores que não estiverem na tabela, serão colocados em hex entre <>
                                        convertido = ("<" + comparador.ToString("X4") + ">");
                                        //convertido = comparador.ToString("X4");       mostra o valor em hex
                                        //comparador.ToString("<" + comparador + ">");  colocar o valor entre <>
                                    }
                                    //A variavel todosOsTextos recebe sempre a letra que acabou de converter
                                    //Após receber a letra q converteu, ele recebe a proxima, sem perder a anterior - essa é a função do sinal +=
                                    todosOsTextos += convertido;
                                }
                            }
                        }
                        //aqui já terminou de ler todos os textos, escreve o TXT com o texto dumpado
                        //File.WriteAllText(Path.GetFileNameWithoutExtension(input) + ".txt", todosOsTextos);
                        File.WriteAllText(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + ".txt", todosOsTextos);

                        todosOsTextos = "";//Avisa que a extração dos textos terminou
                        //Console.WriteLine("Textos extraidos.")
                    }
                }
                MessageBox.Show("Texto Extraido", "AVISO");
            }
        }

        private void inserirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Insersor Megaman X8

            int magic;//Magic do arquivo
            int tamanhoarquivo = 0; //Tamanho do arquivo descrito nele primeira flag depois do Magic
            int game = 0; //PS2 e PC = 0x12 - NSW = 0x1E
            int pp = 0; //proximo ponteiro pra quando estiver inserindo
            int plataforma = ' '; //plataforma do arquivo, PS2 = 1, PC = 2 e NSW = 3 
            long tamanhops2 = 0; //Variavel que ajudará a criar os 0x00 no final do arquivo
            int totaldeponteiros = 0; //Total de ponteiros segundo o arquivo
            int primeiroponteiro;
            int verificador;
            int temnome = 0;


            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Megaman X8 PC/PS2/NSW|*.MCB;*.0589CBA3|Todos os arquivos (*.*)|*.*";
            openFileDialog1.Title = "Abrir arquivo de Megaman X8...";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                foreach (String dump in openFileDialog1.FileNames)
                {

                    using (FileStream stream = File.Open(dump, FileMode.Open))
                    {
                        BinaryReader br = new BinaryReader(stream);
                        BinaryWriter bw = new BinaryWriter(stream);

                        magic = br.ReadInt32(); // Read magic

                        if (magic == 0x2042434D) // Magic PS2 e PC
                        {
                            br.BaseStream.Seek(0x10, SeekOrigin.Begin);

                            tamanhoarquivo = br.ReadInt16(); //Lê a flag de tamanho do arquivo

                            FileInfo fi = new FileInfo(dump);  //informa o tamanho do arquivo em bytes
                            long tamanhototalarquivo = (fi.Length); //tamanho total do arquivo recebe o valor do tamanho


                            if (tamanhototalarquivo > tamanhoarquivo) //se o tamanho total do arquivo for maior que o tamanho declarado na flag
                            {// Arquivo de PS2
                                game = 0x12;
                                plataforma = 1;
                                tamanhops2 = tamanhototalarquivo;
                                //MessageBox.Show("ARQUIVO DE PS2", "My Application", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                            }
                            else
                            {// Arquivo de PC
                                game = 0x12;
                                plataforma = 2;
                                //MessageBox.Show("ARQUIVO DE PC", "My Application", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                            }
                        }
                        else if (magic == 0x42434D4F) //Magic Nintendo Switch
                        {// Arquivo de NSW
                            game = 0x1E;
                            plataforma = 3;
                        }
                        br.BaseStream.Seek(game, SeekOrigin.Begin);
                        totaldeponteiros = br.ReadInt16(); // Lê o valor do total de textos e ponteiros
                        primeiroponteiro = br.ReadInt16(); //Lê o valor do primeiro ponteiro

                        ushort[] ponteiros = new ushort[totaldeponteiros];
                        byte[][] dados = new byte[totaldeponteiros][];
                        byte[][] nomes = new byte[totaldeponteiros][];

                        br.BaseStream.Seek(game + 2 + totaldeponteiros * 2 + primeiroponteiro, SeekOrigin.Begin); //Vai pra posição do primeiro texto
                        verificador = br.ReadUInt16();//Lê o primeiro texto pra ver se é nome ou texto

                        if (verificador <= 0x00FF || verificador == 0xFFFD || verificador == 0xFFFF) //Se for algum desses valores, não tem como ser arquivo com nome
                        {
                            temnome = 0;
                        }
                        else
                        {
                            temnome = 1;

                            br.BaseStream.Seek(game + 2, SeekOrigin.Begin);

                            //ushort[] ponteiros = new ushort[totaldeponteiros];

                            for (int i = 0; i < totaldeponteiros; i++)
                            {
                                ponteiros[i] = br.ReadUInt16();
                            }

                            //byte[][] nomes = new byte[totaldeponteiros][];

                            for (int i = 0; i < totaldeponteiros; i++)
                            {
                                nomes[i] = br.ReadBytes(0x10);
                            }

                            //byte[][] dados = new byte[totaldeponteiros][];

                            for (int i = 0; i < totaldeponteiros; i++)
                            {
                                br.BaseStream.Seek(game + 2 + totaldeponteiros * 2 + totaldeponteiros * 0x10 + ponteiros[i], SeekOrigin.Begin);
                                dados[i] = br.ReadBytes(0x2A);
                            }

                        }
                        br.BaseStream.Seek(game + 2, SeekOrigin.Begin);

                        FileInfo file = new FileInfo(Path.Combine(Path.GetDirectoryName(dump), Path.GetFileNameWithoutExtension(dump)) + ".txt"); // Verifica se o txt existe
                        if (file.Exists)
                        {

                            string txt = File.ReadAllText(Path.Combine(Path.GetDirectoryName(dump), Path.GetFileNameWithoutExtension(dump)) + ".txt"); //se existe ele le o arquivo

                            string[] texto = txt.Split(new string[] { "<end>\r\n" }, StringSplitOptions.RemoveEmptyEntries); //pega o arquivo txt e guarda cada dialogo na variavel texto

                            ushort totaldetextos = (ushort)(texto.Length);//conta quantos textos - Total de textos no arquivo txt

                            stream.SetLength(0x92); //Apaga todos os 0x00 nos arquivos de PS2 pra pegar o tamanho correto do arquivo

                            ushort ponteiro = 0;

                            ushort convertido = ' '; //inicia a variavel que vai converter de ASCII pros caracteres do jogo
                            int pd = 0; //proximos dados

                            for (int dialogo = 0; dialogo < texto.Length; dialogo++)
                            {
                                br.BaseStream.Seek(game + 2 + pp, SeekOrigin.Begin);

                                if (temnome == 1) //tem nome
                                {
                                    bw.Write(ponteiro + pd); //escreve o ponteiro, levando em consideração o espaço dos dados
                                }
                                else //não tem nome
                                {
                                    temnome = 0;
                                    bw.Write(ponteiro); //escreve o ponteiro
                                }

                                for (int c = 0; c < texto[dialogo].Length; c++)
                                {
                                    if (temnome == 1) //tem nome
                                    {
                                        br.BaseStream.Seek(game + 2 + totaldetextos * 2 + ponteiro + totaldetextos * 0x10 + 0x2A + pd, SeekOrigin.Begin);
                                    }
                                    else //não tem nome
                                    {
                                        br.BaseStream.Seek(game + 2 + totaldetextos * 2 + ponteiro, SeekOrigin.Begin);
                                        temnome = 0;
                                    }

                                    char caractere = texto[dialogo][c];

                                    if (caractere >= '0' && caractere <= '9')
                                    {
                                        //Convertendo dentro do intervalo de a e z.
                                        convertido = (ushort)(0x1A + (ushort)(caractere - '0'));
                                    }
                                    else if (caractere >= 'A' && caractere <= 'Z')
                                    {
                                        //Convertendo dentro do intervalo de A e Z.
                                        convertido = (ushort)(0x24 + (ushort)(caractere - 'A'));
                                    }
                                    else if (caractere >= 'a' && caractere <= 'z')
                                    {
                                        //Convertendo dentro do intervalo de a e z.
                                        convertido = (ushort)(0x3E + (ushort)(caractere - 'a'));
                                    }
                                    else if (caractere == ' ')
                                    {
                                        convertido = 0x00;
                                    }
                                    else if (caractere == '!')
                                    {
                                        convertido = 0x01;
                                    }
                                    else if (caractere == '"')
                                    {
                                        convertido = 0x02;
                                    }
                                    else if (caractere == '&')
                                    {
                                        convertido = 0x03;
                                    }
                                    else if (caractere == '%')
                                    {
                                        convertido = 0x04;
                                    }
                                    else if (caractere == '(')
                                    {
                                        convertido = 0x05;
                                    }
                                    else if (caractere == ')')
                                    {
                                        convertido = 0x06;
                                    }
                                    else if (caractere == '+')
                                    {
                                        convertido = 0x08;
                                    }
                                    else if (caractere == '-')
                                    {
                                        convertido = 0x09;
                                    }
                                    else if (caractere == ',')
                                    {
                                        convertido = 0x0A;
                                    }
                                    else if (caractere == '.')
                                    {
                                        convertido = 0x0B;
                                    }
                                    else if (caractere == '/')
                                    {
                                        convertido = 0x0C;
                                    }
                                    else if (caractere == ':')
                                    {
                                        convertido = 0x0D;
                                    }
                                    else if (caractere == ';')
                                    {
                                        convertido = 0x0E;
                                    }
                                    else if (caractere == '=')
                                    {
                                        convertido = 0x0F;
                                    }
                                    else if (caractere == '?')
                                    {
                                        convertido = 0x10;
                                    }
                                    else if (caractere == '\'')
                                    {
                                        convertido = 0x58;
                                    }
                                    else if (caractere == 'À')
                                    {
                                        convertido = 0x9C;
                                    }
                                    else if (caractere == 'Á')
                                    {
                                        convertido = 0x9D;
                                    }
                                    else if (caractere == 'Â')
                                    {
                                        convertido = 0x9E;
                                    }
                                    else if (caractere == 'Ã')
                                    {
                                        convertido = 0x9F;
                                    }
                                    else if (caractere == 'Ä')
                                    {
                                        convertido = 0xA0;
                                    }
                                    else if (caractere == 'Ç')
                                    {
                                        convertido = 0xA3;
                                    }
                                    else if (caractere == 'È')
                                    {
                                        convertido = 0xA4;
                                    }
                                    else if (caractere == 'É')
                                    {
                                        convertido = 0xA5;
                                    }
                                    else if (caractere == 'Ê')
                                    {
                                        convertido = 0xA6;
                                    }
                                    else if (caractere == 'Ë')
                                    {
                                        convertido = 0xA7;
                                    }
                                    else if (caractere == 'Ì')
                                    {
                                        convertido = 0xA8;
                                    }
                                    else if (caractere == 'Í')
                                    {
                                        convertido = 0xA9;
                                    }
                                    else if (caractere == 'Î')
                                    {
                                        convertido = 0xAA;
                                    }
                                    else if (caractere == 'Ï')
                                    {
                                        convertido = 0xAB;
                                    }
                                    else if (caractere == 'Ñ')
                                    {
                                        convertido = 0xAD;
                                    }
                                    else if (caractere == 'Ò')
                                    {
                                        convertido = 0xAE;
                                    }
                                    else if (caractere == 'Ó')
                                    {
                                        convertido = 0xAF;
                                    }
                                    else if (caractere == 'Ô')
                                    {
                                        convertido = 0xB0;
                                    }
                                    else if (caractere == 'Õ')
                                    {
                                        convertido = 0xB1;
                                    }
                                    else if (caractere == 'Ö')
                                    {
                                        convertido = 0xB2;
                                    }
                                    else if (caractere == 'Ù')
                                    {
                                        convertido = 0xB4;
                                    }
                                    else if (caractere == 'Ú')
                                    {
                                        convertido = 0xB5;
                                    }
                                    else if (caractere == 'Û')
                                    {
                                        convertido = 0xB6;
                                    }
                                    else if (caractere == 'Ü')
                                    {
                                        convertido = 0xB7;
                                    }
                                    else if (caractere == 'Ý')
                                    {
                                        convertido = 0xB8;
                                    }
                                    else if (caractere == 'à')
                                    {
                                        convertido = 0xBA;
                                    }
                                    else if (caractere == 'á')
                                    {
                                        convertido = 0xBB;
                                    }
                                    else if (caractere == 'â')
                                    {
                                        convertido = 0xBC;
                                    }
                                    else if (caractere == 'ã')
                                    {
                                        convertido = 0xBD;
                                    }
                                    else if (caractere == 'ä')
                                    {
                                        convertido = 0xBE;
                                    }
                                    else if (caractere == 'ç')
                                    {
                                        convertido = 0xC1;
                                    }
                                    else if (caractere == 'è')
                                    {
                                        convertido = 0xC2;
                                    }
                                    else if (caractere == 'é')
                                    {
                                        convertido = 0xC3;
                                    }
                                    else if (caractere == 'ê')
                                    {
                                        convertido = 0xC4;
                                    }
                                    else if (caractere == 'ë')
                                    {
                                        convertido = 0xC5;
                                    }
                                    else if (caractere == 'ì')
                                    {
                                        convertido = 0xC6;
                                    }
                                    else if (caractere == 'í')
                                    {
                                        convertido = 0xC7;
                                    }
                                    else if (caractere == 'î')
                                    {
                                        convertido = 0xC8;
                                    }
                                    else if (caractere == 'ï')
                                    {
                                        convertido = 0xC9;
                                    }
                                    else if (caractere == 'ñ')
                                    {
                                        convertido = 0xCB;
                                    }
                                    else if (caractere == 'ò')
                                    {
                                        convertido = 0xCC;
                                    }
                                    else if (caractere == 'ó')
                                    {
                                        convertido = 0xCD;
                                    }
                                    else if (caractere == 'ô')
                                    {
                                        convertido = 0xCE;
                                    }
                                    else if (caractere == 'õ')
                                    {
                                        convertido = 0xCF;
                                    }
                                    else if (caractere == 'ö')
                                    {
                                        convertido = 0xD0;
                                    }
                                    else if (caractere == 'ù')
                                    {
                                        convertido = 0xD3;
                                    }
                                    else if (caractere == 'ú')
                                    {
                                        convertido = 0xD4;
                                    }
                                    else if (caractere == 'û')
                                    {
                                        convertido = 0xD5;
                                    }
                                    else if (caractere == 'ü')
                                    {
                                        convertido = 0xD6;
                                    }
                                    else if (caractere == '\r')
                                    {
                                        if (c + 1 < texto[dialogo].Length)
                                        {
                                            caractere = texto[dialogo][c + 1];
                                            if (caractere == '\n') //Se for \n, temos uma sequencia \r\n, pula próximo caractere.
                                            {
                                                c++;
                                            }
                                        }
                                        convertido = 0xFFFD;
                                    }
                                    else if (caractere == '\n')
                                    {
                                        convertido = 0xFFFD;
                                    }

                                    else if (caractere == '<')
                                    {
                                        string outputstring = "";

                                        string entresinal = texto[dialogo];
                                        int inicial = c;
                                        int final = entresinal.IndexOf('>', c + 1);
                                        outputstring = entresinal.Substring(inicial + 1, final - inicial - 1);

                                        if (outputstring == "SINAL_SIGMA")
                                        {
                                            convertido = 0x5A;
                                            c += 12;
                                        }

                                        else
                                        {
                                            ushort numero = Convert.ToUInt16(outputstring, 16);
                                            convertido = numero;
                                            c += 5;
                                        }
                                    }
                                    bw.Write(convertido); //Escreve convertido no MCB etc.
                                    ponteiro += 2;
                                }
                                bw.Write((ushort)0xFFFF); //escreve o endstring
                                ponteiro += 2;
                                pp += 2;
                                pd += 0x2A;
                            }
                            br.BaseStream.Seek(game, SeekOrigin.Begin); //Volta no começo do arquivo
                            bw.Write(totaldetextos);//Escreve o tanto de ponteiros do novo arquivo

                            if (temnome == 1) //tem nome
                            {
                                br.BaseStream.Seek(game + 2, SeekOrigin.Begin);

                                for (int i = 0; i < totaldetextos; i++)
                                {
                                    ponteiros[i] = br.ReadUInt16();
                                }
                                for (int i = 0; i < totaldetextos; i++)
                                {
                                    bw.Write(nomes[i]);
                                }
                                for (int i = 0; i < totaldetextos; i++)
                                {
                                    br.BaseStream.Seek(game + 2 + totaldetextos * 2 + totaldetextos * 0x10 + ponteiros[i], SeekOrigin.Begin);
                                    bw.Write(dados[i]);
                                }
                            }
                        }

                        else
                        {
                            MessageBox.Show("Arquivo TXT não encontrado!", "AVISO");
                            System.Windows.Forms.Application.Restart();
                        }

                    }
                    using (FileStream stream = File.Open(dump, FileMode.Open))
                    {
                        BinaryReader br = new BinaryReader(stream);
                        BinaryWriter bw = new BinaryWriter(stream);

                        if (plataforma == 1) //Arquivo PS2
                        {
                            FileInfo fi = new FileInfo(dump);  //Verifica o tamanho do arquivo em bytes
                            ushort tamanhototalarquivo = Convert.ToUInt16(fi.Length);//tamanho total do arquivo recebe o valor do tamanho
                            bw.BaseStream.Seek(0x10, SeekOrigin.Begin); //Vai para posição para escrever o tamanho do arquivo
                            bw.Write(tamanhototalarquivo); // Escreve o novo tamanho do arquivo
                            stream.SetLength(tamanhops2); //Cria os 0x00 do final do arquivo de PS2
                        }
                        if (plataforma == 2)//Arquivo de PC
                        {
                            FileInfo fi = new FileInfo(dump);  //Verifica o tamanho do arquivo em bytes
                            ushort tamanhototalarquivo = Convert.ToUInt16(fi.Length); //tamanho total do arquivo recebe o valor do tamanho
                            bw.BaseStream.Seek(0x10, SeekOrigin.Begin); //Vai para posição para escrever o tamanho do arquivo
                            bw.Write(tamanhototalarquivo); // Escreve o novo tamanho do arquivo
                        }
                        if (plataforma == 3)//Arquivo de Nintendo Switch
                        {
                            FileInfo fi = new FileInfo(dump);  //Verifica o tamanho do arquivo em bytes
                            ushort tamanhototalarquivo = Convert.ToUInt16(fi.Length); //tamanho total do arquivo recebe o valor do tamanho
                            bw.BaseStream.Seek(0x08, SeekOrigin.Begin); //Vai para posição para escrever o tamanho do arquivo
                            bw.Write(tamanhototalarquivo); // Escreve o novo tamanho do arquivo
                            bw.BaseStream.Seek(0x1C, SeekOrigin.Begin); //Vai para posição para escrever o tamanho do arquivo
                            bw.Write(tamanhototalarquivo); //Escreve o tamanho do arquivo a segunda vez
                        }
                    }
                }
            }


            MessageBox.Show("Texto Inserido", "AVISO");

        }

        private void extrairToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Extrator Megaman X7


            int totaldetextos;
            int ponteiro;
            int comparador;
            string todosOsTextos = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Megaman X7 PC/PS2/NSW|*.BIN;*.06C3DBAA|All files (*.*)|*.*";
            openFileDialog1.Title = "Select a Megaman X7 File";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int numerodearquivosabertos = openFileDialog1.FileNames.Length;

                foreach (String file in openFileDialog1.FileNames)
                {
                    using (BinaryReader br = new BinaryReader(File.OpenRead(file)))

                    {
                        br.BaseStream.Seek(0x00, SeekOrigin.Begin);

                        //Lê o total de Textos
                        totaldetextos = br.ReadInt32();

                        //Pega o total de textos e vai lendo os ponteiros e textos um a um até terminar
                        for (int i = 0; i < totaldetextos; i++)
                        {
                            //Lê o ponteiro
                            ponteiro = ' ';
                            ponteiro = br.ReadInt32();

                            //Inicia a variavel convertido, que fará a conversão dos hex em textos
                            string convertido = "";

                            //vai pro texto
                            br.BaseStream.Seek(4 + totaldetextos * 4 + ponteiro, SeekOrigin.Begin);

                            //Inicia a variavel que vai verificar se o texto acabou ou não
                            bool acabouotexto = false;

                            //Equanto não acabar o texto ele vai repetindo
                            while (acabouotexto == false)
                            {
                                comparador = ' ';
                                //Lê um byte do texto
                                comparador = br.ReadUInt16();

                                //compara se o byte é a endstring
                                //se for, o programa cria uma nova linha
                                //se não continua pra proxima letra
                                if (comparador == 0x8003)
                                {
                                    //Quando chegar em uma endstring ele retorna como o texto tendo acabado (acabouotexto = verdadeiro)
                                    acabouotexto = true;
                                    todosOsTextos += "<end>\r\n";
                                    //Escreve no arquivo todos os textos e quebra a linha sempre em alguma endstring
                                    //File.WriteAllText(Path.GetFileNameWithoutExtension(file) + ".txt", todosOsTextos += "<END>\r\n");
                                }
                                else if (comparador == 0x3FFF)
                                {
                                    //Quando chegar em uma endstring ele retorna como o texto tendo acabado (acabouotexto = verdadeiro)
                                    acabouotexto = true;
                                    todosOsTextos += "<end>\r\n";
                                    //Escreve no arquivo todos os textos e quebra a linha sempre em alguma endstring
                                    //File.WriteAllText(Path.GetFileNameWithoutExtension(file) + ".txt", todosOsTextos += "<END>\r\n");
                                }

                                else
                                {
                                    acabouotexto = false;
                                    //Começa a conversão dos caracteres
                                    if (comparador >= 0x1A && comparador <= 0x23)
                                    {
                                        //Convertendo dentro do intervalo de 0 e 9.
                                        convertido = ((char)('0' + (comparador - 0x1A))).ToString();
                                    }
                                    else if (comparador >= 0x24 && comparador <= 0x3D)
                                    {
                                        //Convertendo dentro do intervalo de A e Z.
                                        convertido = ((char)('A' + (comparador - 0x24))).ToString();
                                    }
                                    else if (comparador >= 0x3E && comparador <= 0x57)
                                    {
                                        //Convertendo dentro do intervalo de a e z.
                                        convertido = ((char)('a' + (comparador - 0x3E))).ToString();
                                    }
                                    else if (comparador == 0x01)
                                    {
                                        convertido = ("!");
                                    }
                                    else if (comparador == 0x02)
                                    {
                                        convertido = '"'.ToString();
                                    }
                                    else if (comparador == 0x05)
                                    {
                                        convertido = ("(");
                                    }
                                    else if (comparador == 0x06)
                                    {
                                        convertido = (")");
                                    }
                                    else if (comparador == 0x08)
                                    {
                                        convertido = ("+");
                                    }
                                    else if (comparador == 0x09)
                                    {
                                        convertido = ("-");
                                    }
                                    else if (comparador == 0x0A)
                                    {
                                        convertido = (",");
                                    }
                                    else if (comparador == 0x0B)
                                    {
                                        convertido = (".");
                                    }
                                    else if (comparador == 0x0C)
                                    {
                                        convertido = ("/");
                                    }
                                    else if (comparador == 0x0D)
                                    {
                                        convertido = (":");
                                    }
                                    else if (comparador == 0x10)
                                    {
                                        convertido = ("?");
                                    }
                                    else if (comparador == 0x58)
                                    {
                                        convertido = ("'");
                                    }
                                    else if (comparador == 0x5A)
                                    {
                                        convertido = ("<SINAL_SIGMA>");
                                    }
                                    else if (comparador == 0x6E)
                                    {
                                        convertido = ("®");
                                    }
                                    else if (comparador == 0x89)
                                    {
                                        convertido = ("É");
                                    }
                                    else if (comparador == 0xA0)
                                    {
                                        convertido = ("à");
                                    }
                                    else if (comparador == 0xA7)
                                    {
                                        convertido = ("ç");
                                    }
                                    else if (comparador == 0xA8)
                                    {
                                        convertido = ("è");
                                    }
                                    else if (comparador == 0xA9)
                                    {
                                        convertido = ("é");
                                    }
                                    else if (comparador == 0xB5)
                                    {
                                        convertido = ("õ");
                                    }
                                    else if (comparador == 0xFF)
                                    {
                                        convertido = ("'");
                                    }
                                    else if (comparador == 0x8002)
                                    {
                                        convertido = ("\r\n");
                                    }
                                    else if (comparador == 0x800C)
                                    {
                                        convertido = (" ");
                                    }
                                    else if (comparador == 0x8004)
                                    {
                                        comparador = br.ReadUInt16();
                                        {
                                            convertido = ("<PAUSA_" + comparador.ToString("X4") + ">");
                                        }
                                    }
                                    else if (comparador == 0x801D)
                                    {
                                        comparador = br.ReadUInt16();
                                        if (comparador == 0x0000)
                                        {
                                            convertido = ("<BOTAO_CIRCULO>");
                                        }
                                        else if (comparador == 0x0001)
                                        {
                                            convertido = ("<BOTAO_QUADRADO>");
                                        }
                                        else if (comparador == 0x0002)
                                        {
                                            convertido = ("<BOTAO_X>");
                                        }
                                        else if (comparador == 0x0003)
                                        {
                                            convertido = ("<BOTAO_TRIANGULO>");
                                        }
                                        else if (comparador == 0x0007)
                                        {
                                            convertido = ("<BOTAO_L2>");
                                        }
                                        else if (comparador == 0x0009)
                                        {
                                            convertido = ("<BOTAO_R2>");
                                        }
                                        else
                                        {
                                            convertido = ("<801D><" + comparador.ToString("X4") + ">");
                                        }
                                    }

                                    else
                                    {
                                        //Os valores que não estiverem na tabela, serão colocados em hex entre <>
                                        convertido = ("<" + comparador.ToString("X4") + ">");
                                        //convertido = comparador.ToString("X4");       mostra o valor em hex
                                        //comparador.ToString("<" + comparador + ">");  colocar o valor entre <>
                                    }

                                    //A variavel todosOsTextos recebe sempre a letra que acabou de converter
                                    //Após receber a letra q converteu, ele recebe a proxima, sem perder a anterior - essa é a função do sinal +=
                                    todosOsTextos += convertido;
                                }
                            }
                            //Volta pra ler o próximo ponteiro
                            br.BaseStream.Seek(0x08 + i * 4, SeekOrigin.Begin);
                        }

                        //aqui já terminou de ler todos os textos, escreve o TXT com o texto dumpado
                        //File.WriteAllText(Path.GetFileNameWithoutExtension(input) + ".txt", todosOsTextos);
                        File.WriteAllText(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + ".txt", todosOsTextos);
                        todosOsTextos = "";
                    }
                }
                MessageBox.Show("Terminado!", "Aviso!");
            }
        }

        private void inserirToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Insersor Megaman X7
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void extrairToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //Extrator X6
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void inserirToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //Insersor Megaman X6
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void extrairToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //Extrator X5
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void inserirToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //Insersor X5
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void extrairToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //Extrator X4
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void inserirToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //Insersor X4
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void extrairToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            //Extrator X3
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void inserirToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            //Insersor X3
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void extrairToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            //Extrator X2
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void inserirToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            //Insersor X2
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void extrairToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            //Extrator X1
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void inserirToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            //Insersor X1
            MessageBox.Show("Em desenvolvimento ainda!", "AVISO!");
        }

        private void extrairToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            //Extrator Command Mission

            int magic;
            int totaldetextos;
            int ponteiro;
            ushort comparador = 0;
            int verificador;
            string todosOsTextos = "";
            int tamanhotexto = 0;
            int tamanhotextoanterior = 0;
            int primeirotexto;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Megaman X Command Mission PS2/NGC|*.*;*.DAT|Todos os arquivos (*.*)|*.*";
            openFileDialog1.Title = "Abrir arquivo de Megaman X Command Mission...";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int numerodearquivosabertos = openFileDialog1.FileNames.Length;

                foreach (String file in openFileDialog1.FileNames)
                {
                    using (BinaryReader br = new BinaryReader(File.OpenRead(file)))

                    {
                        magic = br.ReadInt32(); // Read magic

                        if (magic == 0x00000010) // Magic PS2
                        {

                            //Little Endianess

                        }
                        else if (magic == 0x10000000) //Magic Nintendo GameCube
                        {

                            //Big Endianess

                        }

                        primeirotexto = br.ReadInt16(); //Lê o endereço do primeiro texto

                        br.BaseStream.Seek(0x08, SeekOrigin.Begin);

                        totaldetextos = br.ReadInt16(); // Lê o valor do total de textos
                                                                        
                        for (int i = 0; i < totaldetextos; i++)
                        {
                            br.BaseStream.Seek(0x30 + i * 0x20, SeekOrigin.Begin);

                            ponteiro = br.ReadInt16(); // Lê o valor do ponteiro

                            tamanhotexto = br.ReadInt16(); // Lê o tamanho do texto
                                                        
                            string convertido = ""; //Inicia a variavel convertido, que fará a conversão dos hex em textos

                            br.BaseStream.Seek(primeirotexto + ponteiro * 2, SeekOrigin.Begin);

                            for (int j = 0; j < tamanhotexto; j++)
                            {
                                tamanhotextoanterior = tamanhotexto;

                                //Lê um byte do texto
                                comparador = br.ReadUInt16();
                                
                                //Começa a conversão dos caracteres
                                if (comparador >= 0x92 && comparador <= 0x9B)
                                {
                                    //Convertendo dentro do intervalo de 0 e 9.
                                    convertido = ((char)('0' + (comparador - 0x92))).ToString();
                                }
                                else if (comparador >= 0x9C && comparador <= 0xB5)
                                {
                                    //Convertendo dentro do intervalo de A e Z.
                                    convertido = ((char)('A' + (comparador - 0x9C))).ToString();
                                }
                                else if (comparador >= 0xB6 && comparador <= 0xCF)
                                {
                                    //Convertendo dentro do intervalo de a e z.
                                    convertido = ((char)('a' + (comparador - 0xB6))).ToString();
                                }
                                else if (comparador == 0x8B00)
                                {
                                    convertido = ' '.ToString();
                                }
                                else if (comparador == 0x02)
                                {
                                    convertido = (",");
                                }
                                else if (comparador == 0x03)
                                {
                                    convertido = (".");
                                }
                                else if (comparador == 0x05)
                                {
                                    convertido = (":");
                                }
                                else if (comparador == 0x06)
                                {
                                    convertido = (";");
                                }
                                else if (comparador == 0x07)
                                {
                                    convertido = ("?");
                                }
                                else if (comparador == 0x08)
                                {
                                    convertido = ("!");
                                }
                                else if (comparador == 0x1D)
                                {
                                    convertido = ("/");
                                }
                                else if (comparador == 0x25)
                                {
                                    convertido = ("'");
                                }
                                else if (comparador == 0x27)
                                {
                                    convertido = '"'.ToString();
                                }
                                else if (comparador == 0x28)
                                {
                                    convertido = ("(");
                                }
                                else if (comparador == 0x29)
                                {
                                    convertido = (")");
                                }
                                else if (comparador == 0x3B)
                                {
                                    convertido = ("-");
                                }
                                else if (comparador == 0x8000)
                                {
                                    convertido = ("\r\n");
                                }
                                else
                                {
                                    //Os valores que não estiverem na tabela, serão colocados em hex entre <>
                                    convertido = ("<" + comparador.ToString("X4") + ">");
                                    //convertido = comparador.ToString("X4");       mostra o valor em hex
                                    //comparador.ToString("<" + comparador + ">");  colocar o valor entre <>
                                }
                                
                                //A variavel todosOsTextos recebe sempre a letra que acabou de converter
                                //Após receber a letra q converteu, ele recebe a proxima, sem perder a anterior - essa é a função do sinal +=
                                todosOsTextos += convertido;
                                
                            }

                            
                            todosOsTextos += "<end>\r\n";

                        }
                        //aqui já terminou de ler todos os textos, escreve o TXT com o texto dumpado
                        //File.WriteAllText(Path.GetFileNameWithoutExtension(input) + ".txt", todosOsTextos);
                        File.WriteAllText(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + ".txt", todosOsTextos);

                        todosOsTextos = "";//Avisa que a extração dos textos terminou
                        //Console.WriteLine("Textos extraidos.")
                    }
                }
                MessageBox.Show("Texto Extraido", "AVISO");












            }
        }
    }
}
            