using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MatGo
{
    public partial class WhatooPlayform : Form
    {
        delegate int MyDelegate(string condition, int score);

        //카드 클래스
        public class Card
        {
            public int index; //48개의 카드 구별 인덱스
            public Image image;
            public int month; //카드 월 별 분류
            public int value; //카드의 가치 광=16, 십끗=8, 띠 = 4, 쌍피=2, 피=1

            public Card(int index, int month, Image image, int value)
            {
                setAll(index, month, image, value);
            }
            public Card(int index, int month, Image image)
            {
                setAll(index, month, image, 0);
            }
            public void setAll(int index, int month, Image image, int value)
            {
                this.index = index;
                this.month = month;
                this.image = image;
                this.value = value;
            }

            public void setValue(int value)
            {
                this.value = value;
            }

            public int getMonth()
            {
                return month;
            }

            public Image getImage()
            {
                return image;
            }

            public int getValue()
            {
                return value;
            }

            public int getIndex()
            {
                return index;
            }
        }

        //이긴 경우 표시
        public enum WHOWIN
        {
            USERWIN,
            COMWIN,
            DRAW,
            NULL
        };

        public class CardSetting
        {
            public Card[] AllCard;
            public Random r;
            public int a, b;
            public string[] path;
            public int[] UserCard, ComputerCard, FieldCard, CardDummy;
            public List<int> UserGetCard, ComGetCard;//먹은 카드들 인덱스 저장
            public int[,] cardValueSetting;
            /* 두 리스트는 같이 추가 or 삭제 되므로, 인덱스는 같을것 */
            public List<Card> CardOnBoard;          //필드에 있는 카드들의 실제값(AllCard)정보를 가지고있을 리스트선언
            public List<PictureBox> cardOnField;   //필드에 있는 카드들의 이미지박스 정보를 가지고 있을 리스트 선언
            public int cardNum; // 필드에 전개되어있는 카드 수를 저장할 변수
            public int remain_Card; //더미에 남아있는 카드 수 저장
            public PictureBox temp_drawcard;    //drawCard 함수에서 뽑은 카드를 임의로 저장할 picturebox 변수임
            public Boolean ddong;   //똥쌋는지 여부
            public Boolean turn;    //true : 유저의 턴, false : 컴퓨터의 턴
            public int interval = 13; //카드 겹치는 간격
            public int put_comCard; // 컴퓨터가 놓을 카드의 번호를 저장하는 변수
            public PictureBox[] card_of_Com;    //컴퓨터 패의 픽쳐박스를 없애기위해 이용되는 배열변수
            public int whoChongtong = 0; //유저 총통 시: 1, 컴퓨터 총통 시: 2 저장

            //특정 month의 카드가 몇개 있는지 세는 변수
            public int[] month_count;   // 예를들어, month_count[0] 은 1월인 카드들의 count임
            //scorePanel에 카드들을 겹쳐놓을때 쓸 변수들
            public int p_count = 0;         //피의 개수
            public int pc_p_count = 0;   //컴퓨터의 피의 개수
            public int a_count = 0;         //동물의 개수
            public int pc_a_count = 0;   //컴퓨터의 동물의 개수
            public int d_count = 0;         //띠의 개수
            public int pc_d_count = 0;   //컴퓨터의 띠의 개수
            public int g_count = 0;         //광의 개수
            public int pc_g_count = 0;   //컴퓨터의 광의 개수

            public int user_go = 0;
            public int com_go = 0;

            public int beforeUserScore = 0;
            public int beforeComScore = 0;

            public int[] chongtong;         //유져의 총통 파악을 위한 변수
            public int[] pc_chongtong;   //컴퓨터의 총통 파악을 위한 변수

            public bool godori_trigger = false;
            public bool hongdan_trigger = false;
            public bool chungdan_trigger = false;
            public bool chodan_trigger = false;
            public bool[] gwang_trigger;

            public WHOWIN whoWin = WHOWIN.NULL;
            public int winScore = 0;

            //각 month 별 기준이 되는 좌표값(location)
            public int month1_x = 40, month1_y = 15;
            public int month2_x = 31, month2_y = 160;
            public int month3_x = 50, month3_y = 90;
            public int month4_x = 130, month4_y = 160;
            public int month5_x = 246, month5_y = 160;
            public int month6_x = 350, month6_y = 160;
            public int month7_x = 530, month7_y = 160;
            public int month8_x = 180, month8_y = 77;
            public int month9_x = 451, month9_y = 96;
            public int month10_x = 522, month10_y = 11;
            public int month11_x = 420, month11_y = 11;
            public int month12_x = 300, month12_y = 4;

            //각 피 별 기준이 되는 좌표값
            public int P_user_x = 10, P_user_y = 80;
            public int P_com_x = 36, P_com_y = 2;
            //각 동물 별 기준이 되는 좌표값
            public int A_user_x = 164, A_user_y = 4;
            public int A_com_x = 191, A_com_y = 72;
            //각 띠 별 기준이 되는 좌표값
            public int D_user_x = 367, D_user_y = 4;
            public int D_com_x = 398, D_com_y = 72;
            //각 광 별 기준이 되는 좌표값
            public int G_user_x = 10, G_user_y = 4;
            public int G_com_x = 36, G_com_y = 72;

            public CardSetting()
            {
                AllCard = new Card[48];//카드 선언 위한 클래스 배열

                //CardOnBoard = new Card[20]; //playPanel에 있는 카드들의 AllCard 정보를 담기 위한 클래스 배열
                CardOnBoard = new List<Card>();
                UserGetCard = new List<int>();
                ComGetCard = new List<int>();
                r = new Random();
                path = new string[48];
                //---------------카드를 어떻게 분배했는지 (AllCard배열의 인덱스를 이용해 어떻게 나눴는지) ---------------
                //유저가 선택한 카드의 인덱스는 AllCard의 인덱스 순 일것이다.
                // ex) 유저가 3번째 패를 눌럿을때, 그 3번째 패는 AllCard에선 3번째 카드일 것임
                UserCard = new int[10];
                ComputerCard = new int[10];
                FieldCard = new int[8]; //필드에 깔리는 수
                CardDummy = new int[20]; //카드패
                cardOnField = new List<PictureBox>();
                cardNum = 0;
                //cardOnField_Month = new int[20];
                month_count = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                ddong = false;  //false : 똥 안싼 상태, true : 똥싼 상태
                turn = true;    //처음은 유저의 턴
                remain_Card = 19; // 카드더미에는 19+1장의 카드가 남아있을 것. CardDummy의 인덱스로 쓸 변수임
                put_comCard = 0;    //컴퓨터가 카드를 맨앞부터 차례대로 내게 하기 위한 변수 

                chongtong = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                pc_chongtong = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                gwang_trigger = new bool[3] { false, false, false };

                for (int i = 0; i < 10; i++)
                {
                    UserCard[i] = i;
                    ComputerCard[i] = i + 10;
                }
                for (int y = 0; y < 8; y++)
                {
                    FieldCard[y] = y + 20;
                }
                for (int z = 0; z < 20; z++)
                {
                    CardDummy[z] = z + 28;
                    //cs.AllCard[cs.CardDummy[19]]; CardDummy[19] = 19+28
                }

                //----------------------------------------화투 가치 임시 저장-------------------------------------------
                //카드의 가치 광=16, 십끗=8, 띠 = 4, 쌍피=2, 피=1
                cardValueSetting = new int[,]{
               {16, 4, 1, 1}, {8, 4, 1, 1}, {16, 4, 1, 1}, {8, 4, 1, 1},
               {8, 4, 1, 1}, {8, 4, 1, 1}, {8, 4, 1, 1}, {16, 8, 1, 1},
               {8, 4, 1, 1}, {8, 4, 1, 1}, {16, 2, 1, 1}, {16, 8, 4, 2}
                };

                //각 이미지 불러오기 위한 이미지 별 path 설정
                for (int z = 0; z < 48; z++)
                {
                    path[z] = "./../../../CardImage/" + (z + 1) + ".PNG";

                }

                //카드 48개 선언
                for (int i = 0; i < 48; i++)
                {
                    Image image = Image.FromFile(path[i]);
                    AllCard[i] = new Card(i, (i / 4) + 1, image);
                    //카드 별 가치값
                    AllCard[i].setValue(cardValueSetting[i / 4, i % 4]);
                }

                //--------------------------매번 랜덤하게 카드를 분배하기 위해 AllCard배열 섞기-------------------------------

                //랜덤으로 섞을 때, 사용할 클래스 객체 생성 ;
                Image tempimage = Properties.Resources._50;
                Card temp = new Card(0, 0, tempimage);

                //AllCard배열 500번 섞어주기
                for (int y = 0; y < 500; y++)
                {

                    a = r.Next(0, 48);
                    b = r.Next(0, 48);
                    temp = AllCard[a];
                    AllCard[a] = AllCard[b];
                    AllCard[b] = temp;
                }

            }//void CardSetting()
        }//class CardSetting

        public CardSetting cs;

        public void cardOnField()
        {
            PictureBox fieldcard1 = new PictureBox();



            if (cs.AllCard[cs.FieldCard[0]].month == 1)
            {
                cs.month_count[0] += 1;
                fieldcard1.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);    //필드에 깔린 첫번째 카드의 정보를 등록함

            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 2)
            {
                cs.month_count[1] += 1;
                fieldcard1.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 3)
            {
                cs.month_count[2] += 1;
                fieldcard1.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 4)
            {
                cs.month_count[3] += 1;
                fieldcard1.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 5)
            {
                cs.month_count[4] += 1;
                fieldcard1.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 6)
            {
                cs.month_count[5] += 1;
                fieldcard1.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 7)
            {
                cs.month_count[6] += 1;
                fieldcard1.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 8)
            {
                cs.month_count[7] += 1;
                fieldcard1.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 9)
            {
                cs.month_count[8] += 1;
                fieldcard1.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 10)
            {
                cs.month_count[9] += 1;
                fieldcard1.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 11)
            {
                cs.month_count[10] += 1;
                fieldcard1.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            else if (cs.AllCard[cs.FieldCard[0]].month == 12)
            {
                cs.month_count[11] += 1;
                fieldcard1.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[0]]);
            }
            fieldcard1.Size = new Size(40, 63);
            fieldcard1.Image = cs.AllCard[cs.FieldCard[0]].image;

            //PlayPanel에 놓인 카드의 픽쳐박스 저장후 인덱스 증가
            cs.cardOnField.Add(fieldcard1);
            cs.cardNum++;

            PictureBox fieldcard2 = new PictureBox();

            if (cs.AllCard[cs.FieldCard[1]].month == 1)
            {
                cs.month_count[0] += 1;
                fieldcard2.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 2)
            {
                cs.month_count[1] += 1;
                fieldcard2.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 3)
            {
                cs.month_count[2] += 1;
                fieldcard2.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 4)
            {
                cs.month_count[3] += 1;
                fieldcard2.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 5)
            {
                cs.month_count[4] += 1;
                fieldcard2.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 6)
            {
                cs.month_count[5] += 1;
                fieldcard2.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 7)
            {
                cs.month_count[6] += 1;
                fieldcard2.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 8)
            {
                cs.month_count[7] += 1;
                fieldcard2.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 9)
            {
                cs.month_count[8] += 1;
                fieldcard2.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 10)
            {
                cs.month_count[9] += 1;
                fieldcard2.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 11)
            {
                cs.month_count[10] += 1;
                fieldcard2.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            else if (cs.AllCard[cs.FieldCard[1]].month == 12)
            {
                cs.month_count[11] += 1;
                fieldcard2.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[1]]);
            }
            fieldcard2.Size = new Size(40, 63);
            fieldcard2.Image = cs.AllCard[cs.FieldCard[1]].image;

            cs.cardOnField.Add(fieldcard2);
            cs.cardNum++;

            PictureBox fieldcard3 = new PictureBox();
            if (cs.AllCard[cs.FieldCard[2]].month == 1)
            {
                cs.month_count[0] += 1;
                fieldcard3.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 2)
            {
                cs.month_count[1] += 1;
                fieldcard3.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 3)
            {
                cs.month_count[2] += 1;
                fieldcard3.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 4)
            {
                cs.month_count[3] += 1;
                fieldcard3.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 5)
            {
                cs.month_count[4] += 1;
                fieldcard3.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 6)
            {
                cs.month_count[5] += 1;
                fieldcard3.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 7)
            {
                cs.month_count[6] += 1;
                fieldcard3.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 8)
            {
                cs.month_count[7] += 1;
                fieldcard3.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 9)
            {
                cs.month_count[8] += 1;
                fieldcard3.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 10)
            {
                cs.month_count[9] += 1;
                fieldcard3.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 11)
            {
                cs.month_count[10] += 1;
                fieldcard3.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            else if (cs.AllCard[cs.FieldCard[2]].month == 12)
            {
                cs.month_count[11] += 1;
                fieldcard3.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[2]]);
            }
            fieldcard3.Size = new Size(40, 63);
            fieldcard3.Image = cs.AllCard[cs.FieldCard[2]].image;

            cs.cardOnField.Add(fieldcard3);
            cs.cardNum++;

            PictureBox fieldcard4 = new PictureBox();
            if (cs.AllCard[cs.FieldCard[3]].month == 1)
            {
                cs.month_count[0] += 1;
                fieldcard4.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 2)
            {
                cs.month_count[1] += 1;
                fieldcard4.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 3)
            {
                cs.month_count[2] += 1;
                fieldcard4.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 4)
            {
                cs.month_count[3] += 1;
                fieldcard4.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 5)
            {
                cs.month_count[4] += 1;
                fieldcard4.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 6)
            {
                cs.month_count[5] += 1;
                fieldcard4.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 7)
            {
                cs.month_count[6] += 1;
                fieldcard4.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 8)
            {
                cs.month_count[7] += 1;
                fieldcard4.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 9)
            {
                cs.month_count[8] += 1;
                fieldcard4.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 10)
            {
                cs.month_count[9] += 1;
                fieldcard4.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 11)
            {
                cs.month_count[10] += 1;
                fieldcard4.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            else if (cs.AllCard[cs.FieldCard[3]].month == 12)
            {
                cs.month_count[11] += 1;
                fieldcard4.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[3]]);
            }
            fieldcard4.Size = new Size(40, 63);
            fieldcard4.Image = cs.AllCard[cs.FieldCard[3]].image;

            cs.cardOnField.Add(fieldcard4);
            cs.cardNum++;


            PictureBox fieldcard5 = new PictureBox();
            if (cs.AllCard[cs.FieldCard[4]].month == 1)
            {
                cs.month_count[0] += 1;
                fieldcard5.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 2)
            {
                cs.month_count[1] += 1;
                fieldcard5.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 3)
            {
                cs.month_count[2] += 1;
                fieldcard5.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 4)
            {
                cs.month_count[3] += 1;
                fieldcard5.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 5)
            {
                cs.month_count[4] += 1;
                fieldcard5.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 6)
            {
                cs.month_count[5] += 1;
                fieldcard5.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 7)
            {
                cs.month_count[6] += 1;
                fieldcard5.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 8)
            {
                cs.month_count[7] += 1;
                fieldcard5.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 9)
            {
                cs.month_count[8] += 1;
                fieldcard5.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 10)
            {
                cs.month_count[9] += 1;
                fieldcard5.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 11)
            {
                cs.month_count[10] += 1;
                fieldcard5.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            else if (cs.AllCard[cs.FieldCard[4]].month == 12)
            {
                cs.month_count[11] += 1;
                fieldcard5.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[4]]);
            }
            fieldcard5.Size = new Size(40, 63);
            fieldcard5.Image = cs.AllCard[cs.FieldCard[4]].image;

            cs.cardOnField.Add(fieldcard5);
            cs.cardNum++;

            PictureBox fieldcard6 = new PictureBox();
            if (cs.AllCard[cs.FieldCard[5]].month == 1)
            {
                cs.month_count[0] += 1;
                fieldcard6.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 2)
            {
                cs.month_count[1] += 1;
                fieldcard6.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 3)
            {
                cs.month_count[2] += 1;
                fieldcard6.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 4)
            {
                cs.month_count[3] += 1;
                fieldcard6.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 5)
            {
                cs.month_count[4] += 1;
                fieldcard6.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 6)
            {
                cs.month_count[5] += 1;
                fieldcard6.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 7)
            {
                cs.month_count[6] += 1;
                fieldcard6.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 8)
            {
                cs.month_count[7] += 1;
                fieldcard6.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 9)
            {
                cs.month_count[8] += 1;
                fieldcard6.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 10)
            {
                cs.month_count[9] += 1;
                fieldcard6.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 11)
            {
                cs.month_count[10] += 1;
                fieldcard6.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            else if (cs.AllCard[cs.FieldCard[5]].month == 12)
            {
                cs.month_count[11] += 1;
                fieldcard6.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[5]]);
            }
            fieldcard6.Size = new Size(40, 63);
            fieldcard6.Image = cs.AllCard[cs.FieldCard[5]].image;

            cs.cardOnField.Add(fieldcard6);
            cs.cardNum++;

            PictureBox fieldcard7 = new PictureBox();
            if (cs.AllCard[cs.FieldCard[6]].month == 1)
            {
                cs.month_count[0] += 1;
                fieldcard7.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 2)
            {
                cs.month_count[1] += 1;
                fieldcard7.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);

                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 3)
            {
                cs.month_count[2] += 1;
                fieldcard7.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 4)
            {
                cs.month_count[3] += 1;
                fieldcard7.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 5)
            {
                cs.month_count[4] += 1;
                fieldcard7.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 6)
            {
                cs.month_count[5] += 1;
                fieldcard7.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 7)
            {
                cs.month_count[6] += 1;
                fieldcard7.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 8)
            {
                cs.month_count[7] += 1;
                fieldcard7.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 9)
            {
                cs.month_count[8] += 1;
                fieldcard7.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 10)
            {
                cs.month_count[9] += 1;
                fieldcard7.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 11)
            {
                cs.month_count[10] += 1;
                fieldcard7.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            else if (cs.AllCard[cs.FieldCard[6]].month == 12)
            {
                cs.month_count[11] += 1;
                fieldcard7.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[6]]);
            }
            fieldcard7.Size = new Size(40, 63);
            fieldcard7.Image = cs.AllCard[cs.FieldCard[6]].image;

            cs.cardOnField.Add(fieldcard7);
            cs.cardNum++;

            PictureBox fieldcard8 = new PictureBox();
            if (cs.AllCard[cs.FieldCard[7]].month == 1)
            {
                cs.month_count[0] += 1;
                fieldcard8.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 2)
            {
                cs.month_count[1] += 1;
                fieldcard8.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 3)
            {
                cs.month_count[2] += 1;
                fieldcard8.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 4)
            {
                cs.month_count[3] += 1;
                fieldcard8.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 5)
            {
                cs.month_count[4] += 1;
                fieldcard8.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 6)
            {
                cs.month_count[5] += 1;
                fieldcard8.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 7)
            {
                cs.month_count[6] += 1;
                fieldcard8.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 8)
            {
                cs.month_count[7] += 1;
                fieldcard8.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 9)
            {
                cs.month_count[8] += 1;
                fieldcard8.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 10)
            {
                cs.month_count[9] += 1;
                fieldcard8.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 11)
            {
                cs.month_count[10] += 1;
                fieldcard8.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            else if (cs.AllCard[cs.FieldCard[7]].month == 12)
            {
                cs.month_count[11] += 1;
                fieldcard8.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);
                cs.CardOnBoard.Add(cs.AllCard[cs.FieldCard[7]]);
            }
            fieldcard8.Size = new Size(40, 63);
            fieldcard8.Image = cs.AllCard[cs.FieldCard[7]].image;

            cs.cardOnField.Add(fieldcard8);
            cs.cardNum++;

            PlayPanel.Controls.Add(fieldcard1);
            fieldcard1.BringToFront();
            PlayPanel.Controls.Add(fieldcard2);
            fieldcard2.BringToFront();
            PlayPanel.Controls.Add(fieldcard3);
            fieldcard3.BringToFront();
            PlayPanel.Controls.Add(fieldcard4);
            fieldcard4.BringToFront();
            PlayPanel.Controls.Add(fieldcard5);
            fieldcard5.BringToFront();
            PlayPanel.Controls.Add(fieldcard6);
            fieldcard6.BringToFront();
            PlayPanel.Controls.Add(fieldcard7);
            fieldcard7.BringToFront();
            PlayPanel.Controls.Add(fieldcard8);
            fieldcard8.BringToFront();

            //필드에 같은 월이 4장 깔릴 때, 유저가 4장을 다 가져옴
            for (int k = 1; k <= 12; k++)
            {
                if (cs.month_count[k - 1] == 4)
                {
                    for (int j = 0; j < 4; j++)
                    {   // 4장을 지워야하기때문에 같은로직 4번 돌림

                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            if (cs.CardOnBoard[p - 1].month == k)
                            {

                                //해당 인덱스 저장
                                cs.UserGetCard.Add(cs.CardOnBoard[p - 1].index);


                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_user_x + cs.interval * cs.p_count, cs.P_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.p_count += 2;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_user_x + cs.interval * cs.d_count, cs.D_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.d_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_user_x + cs.interval * cs.a_count, cs.A_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.a_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_user_x + cs.interval * cs.g_count, cs.G_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.g_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                        }//for i

                    }//for j
                }//if
            }//for


        }

        public WhatooPlayform()
        {


            InitializeComponent();


            // 이미지 파일 예외처리
            try
            {
                cs = new CardSetting();
            }
            catch (Exception exp)
            {
                MessageBox.Show("ERROR : 이미지 파일에 문제가 생겼습니다.");
                //이미지 파일중에 문제가 생기면 에러메시지를 출력하고 프로그램을 종료함
                Application.Exit();
            }


            //컴퓨터가 카드를 낼때 낸 카드를 없애기 위해서 배열안에 저장함
            cs.card_of_Com = new PictureBox[10] { computerCard1, computerCard2, computerCard3, computerCard4, computerCard5, computerCard6, computerCard7, computerCard8, computerCard9, computerCard10 };
            //-------------------------------------------PlayPanel에 카드 8장 깔기----------------------------------------
            cardOnField();

            //------------------------------------userCard 버튼 이미지 설정(카드패 분배)------------------------------------
            userCard1.BackgroundImage = cs.AllCard[cs.UserCard[0]].image;
            userCard1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            userCard2.BackgroundImage = cs.AllCard[cs.UserCard[1]].image;
            userCard2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            userCard3.BackgroundImage = cs.AllCard[cs.UserCard[2]].image;
            userCard3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            userCard4.BackgroundImage = cs.AllCard[cs.UserCard[3]].image;
            userCard4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            userCard5.BackgroundImage = cs.AllCard[cs.UserCard[4]].image;
            userCard5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            userCard6.BackgroundImage = cs.AllCard[cs.UserCard[5]].image;
            userCard6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            userCard7.BackgroundImage = cs.AllCard[cs.UserCard[6]].image;
            userCard7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            userCard8.BackgroundImage = cs.AllCard[cs.UserCard[7]].image;
            userCard8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            userCard9.BackgroundImage = cs.AllCard[cs.UserCard[8]].image;
            userCard9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            userCard10.BackgroundImage = cs.AllCard[cs.UserCard[9]].image;
            userCard10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            checkChongtong();   //총통 발생 여부 체크
            
        }

        //모든 카드 분배 후, 총통 파악하는 로직
        public void checkChongtong()
        {
            for (int ch = 0; ch < 10; ch++)
            {
                if (cs.AllCard[cs.UserCard[ch]].month == 1)
                    cs.chongtong[0]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 1)
                    cs.pc_chongtong[0]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 2)
                    cs.chongtong[1]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 2)
                    cs.pc_chongtong[1]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 3)
                    cs.chongtong[2]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 3)
                    cs.pc_chongtong[2]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 4)
                    cs.chongtong[3]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 4)
                    cs.pc_chongtong[3]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 5)
                    cs.chongtong[4]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 5)
                    cs.pc_chongtong[4]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 6)
                    cs.chongtong[5]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 6)
                    cs.pc_chongtong[5]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 7)
                    cs.chongtong[6]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 7)
                    cs.pc_chongtong[6]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 8)
                    cs.chongtong[7]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 8)
                    cs.pc_chongtong[7]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 9)
                    cs.chongtong[8]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 9)
                    cs.pc_chongtong[8]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 10)
                    cs.chongtong[9]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 10)
                    cs.pc_chongtong[9]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 11)
                    cs.chongtong[10]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 11)
                    cs.pc_chongtong[10]++;

                if (cs.AllCard[cs.UserCard[ch]].month == 12)
                    cs.chongtong[11]++;
                else if (cs.AllCard[cs.ComputerCard[ch]].month == 12)
                    cs.pc_chongtong[11]++;
            }

            for (int ch = 0; ch < 12; ch++)
            {
                if (cs.chongtong[ch] == 4)
                {
                    MessageBox.Show("유저의 총통입니다. 유저의 승리입니다.");

                    cs.whoChongtong = 1;
                    cs.whoWin = WHOWIN.USERWIN;
                    cs.winScore = 50; //총통일 시 50점으로 게임 끝
                    showScoreForm();
                }
                else if (cs.pc_chongtong[ch] == 4)
                {
                    MessageBox.Show("컴퓨터의 총통입니다. 컴퓨터의 승리입니다.");

                    cs.whoChongtong = 2;
                    cs.whoWin = WHOWIN.COMWIN;
                    cs.winScore = 50;
                    showScoreForm();
                }
            }
        }

        // 딜레이를 주기 위한 함수 정의
        public static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }

        // 한 플레이어가 낸 카드를 필드에 전개되어있는 카드중에서 비교하여 겹치거나, 그냥 놓음
        public void compare_userCard(PictureBox spendCard, int num)
        {
            if (cs.AllCard[cs.UserCard[num]].month == 1)
            {
                cs.month_count[0] += 1;
                spendCard.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 2)
            {
                cs.month_count[1] += 1;
                spendCard.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 3)
            {
                cs.month_count[2] += 1;
                spendCard.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 4)
            {
                cs.month_count[3] += 1;
                spendCard.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 5)
            {
                cs.month_count[4] += 1;
                spendCard.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 6)
            {
                cs.month_count[5] += 1;
                spendCard.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 7)
            {
                cs.month_count[6] += 1;
                spendCard.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 8)
            {
                cs.month_count[7] += 1;
                spendCard.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 9)
            {
                cs.month_count[8] += 1;
                spendCard.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 10)
            {
                cs.month_count[9] += 1;
                spendCard.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 11)
            {
                cs.month_count[10] += 1;
                spendCard.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);
            }
            else if (cs.AllCard[cs.UserCard[num]].month == 12)
            {
                cs.month_count[11] += 1;
                spendCard.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);
            }
        }

        public void compare_comCard(PictureBox spendCard, int num)
        {
            if (cs.AllCard[cs.ComputerCard[num]].month == 1)
            {
                cs.month_count[0] += 1;
                spendCard.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 2)
            {
                cs.month_count[1] += 1;
                spendCard.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 3)
            {
                cs.month_count[2] += 1;
                spendCard.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 4)
            {
                cs.month_count[3] += 1;
                spendCard.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 5)
            {
                cs.month_count[4] += 1;
                spendCard.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 6)
            {
                cs.month_count[5] += 1;
                spendCard.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 7)
            {
                cs.month_count[6] += 1;
                spendCard.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 8)
            {
                cs.month_count[7] += 1;
                spendCard.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 9)
            {
                cs.month_count[8] += 1;
                spendCard.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 10)
            {
                cs.month_count[9] += 1;
                spendCard.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 11)
            {
                cs.month_count[10] += 1;
                spendCard.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);
            }
            else if (cs.AllCard[cs.ComputerCard[num]].month == 12)
            {
                cs.month_count[11] += 1;
                spendCard.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);
            }
        }

        public void compare_drawCard(PictureBox spendCard, int num)
        {
            if (cs.AllCard[cs.CardDummy[num]].month == 1)
            {
                cs.month_count[0] += 1;
                spendCard.Location = new Point(cs.month1_x + cs.interval * cs.month_count[0], cs.month1_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 2)
            {
                cs.month_count[1] += 1;
                spendCard.Location = new Point(cs.month2_x + cs.interval * cs.month_count[1], cs.month2_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 3)
            {
                cs.month_count[2] += 1;
                spendCard.Location = new Point(cs.month3_x + cs.interval * cs.month_count[2], cs.month3_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 4)
            {
                cs.month_count[3] += 1;
                spendCard.Location = new Point(cs.month4_x + cs.interval * cs.month_count[3], cs.month4_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 5)
            {
                cs.month_count[4] += 1;
                spendCard.Location = new Point(cs.month5_x + cs.interval * cs.month_count[4], cs.month5_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 6)
            {
                cs.month_count[5] += 1;
                spendCard.Location = new Point(cs.month6_x + cs.interval * cs.month_count[5], cs.month6_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 7)
            {
                cs.month_count[6] += 1;
                spendCard.Location = new Point(cs.month7_x + cs.interval * cs.month_count[6], cs.month7_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 8)
            {
                cs.month_count[7] += 1;
                spendCard.Location = new Point(cs.month8_x + cs.interval * cs.month_count[7], cs.month8_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 9)
            {
                cs.month_count[8] += 1;
                spendCard.Location = new Point(cs.month9_x + cs.interval * cs.month_count[8], cs.month9_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 10)
            {
                cs.month_count[9] += 1;
                spendCard.Location = new Point(cs.month10_x + cs.interval * cs.month_count[9], cs.month10_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 11)
            {
                cs.month_count[10] += 1;
                spendCard.Location = new Point(cs.month11_x + cs.interval * cs.month_count[10], cs.month11_y);
            }
            else if (cs.AllCard[cs.CardDummy[num]].month == 12)
            {
                cs.month_count[11] += 1;
                spendCard.Location = new Point(cs.month12_x + cs.interval * cs.month_count[11], cs.month12_y);
            }
        }

        //한 플레이어가 자신의 패를 한장 냈으면 카드더미에서 한장 뽑아 필드에 전개하는 함수
        public void draw_Card()
        {
            //카드더미에서 카드를 뽑을땐, 더미의 마지막 카드부터 차례대로 뽑는다
            PictureBox drawed_Card = new PictureBox();
            //뽑은 카드의 이미지는 카드더미의 마지막에 있는 카드를 사용한다.
            drawed_Card.Image = cs.AllCard[cs.CardDummy[cs.remain_Card]].image;
            //뽑은 카드의 월을 이용하여 비교한다.
            compare_drawCard(drawed_Card, cs.remain_Card);
            drawed_Card.Size = new Size(40, 63);
            PlayPanel.Controls.Add(drawed_Card);
            drawed_Card.BringToFront();
            cs.cardOnField.Add(drawed_Card);
            cs.CardOnBoard.Add(cs.AllCard[cs.CardDummy[cs.remain_Card]]);
            cs.cardNum++;
            cs.temp_drawcard = drawed_Card; // 더미에서 뽑은 카드를 임시로 저장함 -> scorePanel로의 연산에 사용됨
            //카드를 뽑았으니, CardDummy의 배열인덱스를 하나 줄임
            cs.remain_Card--;

            if (cs.remain_Card == -1)
            {   //카드더미의 카드가 없으면, 가운데에 카드더미를 상징하고 있던 카드 뒷면을 없앰
                pictureBox1.Enabled = false;
                pictureBox1.Visible = false;
            }
        }

        public void com_PutCardDown() // 컴퓨터가 카드를 내는 함수
        {

            while (cs.turn == false) // 유저 턴 끝나면 자동 실행
            {
                PictureBox spendCard = new PictureBox();
                spendCard.Image = cs.AllCard[cs.ComputerCard[cs.put_comCard]].image;
                compare_comCard(spendCard, cs.put_comCard);
                spendCard.Size = new Size(40, 63);
                PlayPanel.Controls.Add(spendCard);

                //컴퓨터가 낸 카드를 computerPanel에서 안보이게 해줌
                cs.card_of_Com[cs.put_comCard].Enabled = false;
                cs.card_of_Com[cs.put_comCard].Visible = false;
                spendCard.BringToFront();
                cs.cardOnField.Add(spendCard);
                cs.CardOnBoard.Add(cs.AllCard[cs.ComputerCard[cs.put_comCard]]);
                cs.cardNum++;

                Delay(300);
                draw_Card();
                Delay(300);
                setComCardOnScorePanel(spendCard);  //컴퓨터가 낸 카드를 컴퓨터의 스코어 패널에 가져옴
                Delay(300);
                setDrawComCardOnScorePanel();           //컴퓨터가 드로우 해서 먹은 카드들을 컴퓨터의 스코어 패널에 가져옴
                Delay(300);


                cs.put_comCard++;   //컴퓨터가 맨 앞 카드를 냈으니, 다음 카드를 낼 차례를 암시해줌
               
                checkWin();
                cs.turn = true; //컴퓨터가 할일을 했으니 유저에게 턴을 넘김

            }
        }

        //유저가 낸 카드랑 겹쳤던 카드들을 유저의 스코어패널에 놓는 함수
        public void setUserCardOnScorePanel(PictureBox spendCard, int num)
        {
            for (int k = 1; k <= 12; k++)
            {
                //똥쌌을때 처리 -> 패를 안가져옴
                //유저가 낸 카드의 월과 더미에서 뽑은 카드의 월이 같고, 유저가 낸 카드의 월 카운트가 3개일때
                if (cs.AllCard[cs.UserCard[num]].month == cs.AllCard[cs.CardDummy[cs.remain_Card + 1]].month &&
                    cs.AllCard[cs.UserCard[num]].month == k && cs.month_count[k - 1] == 3)
                {
                    cs.ddong = true;
                    //쌌을때, 똥그림이 2초동안 나타났다가 없어짐
                    PictureBox ddong = new PictureBox();
                    ddong.BackgroundImage = Properties.Resources.main2;
                    ddong.Image = Properties.Resources.bbuk;
                    ddong.Location = new Point(265, 77);
                    ddong.Size = new Size(80, 77);
                    PlayPanel.Controls.Add(ddong);
                    ddong.BringToFront();
                    Delay(2000);
                    PlayPanel.Controls.Remove(ddong);
                    return;
                }
            }

            for (int k = 1; k <= 12; k++)
            {
                //유저가 k월 카드를 냈는데, k월의 카운트가 2일때 -> 먹어야함
                if (cs.AllCard[cs.UserCard[num]].month == k && cs.month_count[k - 1] == 2)
                {


                    //k월인 2장의 카드의 값과 이미지를 캐치해야함

                    for (int j = 0; j < 2; j++)
                    {   // 2장을 지워야하기때문에 같은로직 2번 돌림
                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                
                                //해당 인덱스 저장
                                cs.UserGetCard.Add(cs.CardOnBoard[p - 1].index);
                                

                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_user_x + cs.interval * cs.p_count, cs.P_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.p_count += 2;

                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_user_x + cs.interval * cs.d_count, cs.D_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.d_count++;
                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_user_x + cs.interval * cs.a_count, cs.A_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.a_count++;
                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_user_x + cs.interval * cs.g_count, cs.G_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.g_count++;
                                    cs.month_count[k - 1]--;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                        }//for i
                    }//for j
                }
                else if (cs.AllCard[cs.UserCard[num]].month == k && cs.month_count[k - 1] == 3 && cs.ddong == false)
                {
                    //필드에 2장 깔려있는 카드의 월을 냈을때, 내가 낸것과 깔려있었던 둘중 하나의 것을 가져옴



                    for (int j = 0; j < 2; j++)
                    {   // 2장을 지워야하기때문에 같은로직 2번 돌림

                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                
                                //해당 인덱스 저장
                                cs.UserGetCard.Add(cs.CardOnBoard[p - 1].index);
                                

                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_user_x + cs.interval * cs.p_count, cs.P_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.p_count += 2;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_user_x + cs.interval * cs.d_count, cs.D_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.d_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_user_x + cs.interval * cs.a_count, cs.A_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.a_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_user_x + cs.interval * cs.g_count, cs.G_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.g_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                            if (cs.month_count[k - 1] == 1)
                                return;
                        }//for i

                    }//for j

                }
                else if (cs.AllCard[cs.UserCard[num]].month == k && cs.month_count[k - 1] == 4)
                {
                    //필드에 3장 깔려있는 카드의 월을 냈을때, 4장 다 가져옴

                    for (int j = 0; j < 4; j++)
                    {   // 4장을 지워야하기때문에 같은로직 4번 돌림

                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            
                            if (cs.CardOnBoard[p - 1].month == k)
                            {

                                
                                //해당 인덱스 저장
                                cs.UserGetCard.Add(cs.CardOnBoard[p - 1].index);
                                

                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_user_x + cs.interval * cs.p_count, cs.P_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.p_count += 2;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_user_x + cs.interval * cs.d_count, cs.D_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.d_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_user_x + cs.interval * cs.a_count, cs.A_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.a_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_user_x + cs.interval * cs.g_count, cs.G_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.g_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                        }//for i

                    }//for j

                }//if-else-if
            }//for k
        }// setUserCardOnScorePanel()

        //유저가 뽑은 카드랑 겹쳤던 카드들을 유저의 스코어 패널에 놓는 함수
        public void setDrawCardOnScorePanel()
        {
            //유저가 똥쌌으면 아무것도 하지않고 넘김
            if (cs.ddong == true)
            {
                cs.ddong = false;
                return;
            }




            for (int k = 1; k <= 12; k++)
            {
                //유저가 k월 카드를 냈는데, k월의 카운트가 2일때 -> 먹어야함
                if (cs.AllCard[cs.CardDummy[cs.remain_Card + 1]].month == k && cs.month_count[k - 1] == 2)
                {
                    //k월인 2장의 카드의 값과 이미지를 캐치해야함

                    for (int j = 0; j < 2; j++)
                    {   // 2장을 지워야하기때문에 같은로직 2번 돌림
                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            
                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                //해당 인덱스 저장
                                cs.UserGetCard.Add(cs.CardOnBoard[p - 1].index);



                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_user_x + cs.interval * cs.p_count, cs.P_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.p_count += 2;
                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_user_x + cs.interval * cs.d_count, cs.D_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.d_count++;
                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_user_x + cs.interval * cs.a_count, cs.A_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.a_count++;
                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_user_x + cs.interval * cs.g_count, cs.G_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.g_count++;
                                    cs.month_count[k - 1]--;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                        }//for i
                    }//for j
                    break;
                }
                else if (cs.AllCard[cs.CardDummy[cs.remain_Card + 1]].month == k && cs.month_count[k - 1] == 3 && cs.ddong == false)
                {
                    //필드에 2장 깔려있는 카드의 월을 냈을때, 내가 낸것과 깔려있었던 둘중 하나의 것을 가져옴

                    for (int j = 0; j < 2; j++)
                    {   // 2장을 지워야하기때문에 같은로직 2번 돌림

                        for (int p = cs.cardNum; p >= 1; p--)
                        {

                           
                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                
                                //해당 인덱스 저장
                                cs.UserGetCard.Add(cs.CardOnBoard[p - 1].index);
                                

                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_user_x + cs.interval * cs.p_count, cs.P_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.p_count += 2;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_user_x + cs.interval * cs.d_count, cs.D_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.d_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_user_x + cs.interval * cs.a_count, cs.A_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.a_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_user_x + cs.interval * cs.g_count, cs.G_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.g_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                            if (cs.month_count[k - 1] == 1)
                                return;
                        }//for i
                    }//for j
                    break;
                }
                else if (cs.AllCard[cs.CardDummy[cs.remain_Card + 1]].month == k && cs.month_count[k - 1] == 4)
                {
                    //뽑은 카드가 필드에 3장 깔려있는 카드의 월을 가지는 카드일때, 4장 다가져옴

                    for (int j = 0; j < 4; j++)
                    {   // 4장을 지워야하기때문에 같은로직 4번 돌림

                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            //해당 인덱스 저장
                            cs.UserGetCard.Add(cs.CardOnBoard[p - 1].index);

                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                /*
                                //해당 인덱스 저장
                                cs.UserGetCard.Add(cs.CardOnBoard[p - 1].index);
                                */

                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_user_x + cs.interval * cs.p_count, cs.P_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.p_count += 2;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_user_x + cs.interval * cs.d_count, cs.D_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.d_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_user_x + cs.interval * cs.a_count, cs.A_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.a_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_user_x + cs.interval * cs.g_count, cs.G_user_y);
                                    userScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.g_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                        }//for i
                    }//for j
                    break;
                }//if-else-if
            }//for k
            

        }// setDrawCardOnScorePanel()

        //컴퓨터가 낸 카드랑 겹쳤던 카드들을 컴퓨터의 스코어패널에 놓는 함수
        public void setComCardOnScorePanel(PictureBox spendCard)
        {
            for (int k = 1; k <= 12; k++)
            {
                //똥쌌을때 처리 -> 패를 안가져옴
                //유저가 낸 카드의 월과 더미에서 뽑은 카드의 월이 같고, 유저가 낸 카드의 월 카운트가 3개일때
                if (cs.AllCard[cs.ComputerCard[cs.put_comCard]].month == cs.AllCard[cs.CardDummy[cs.remain_Card + 1]].month &&
                    cs.AllCard[cs.ComputerCard[cs.put_comCard]].month == k && cs.month_count[k - 1] == 3)
                {
                    cs.ddong = true;
                    PictureBox ddong = new PictureBox();
                    ddong.BackgroundImage = Properties.Resources.main2;
                    ddong.Image = Properties.Resources.bbuk;
                    ddong.Location = new Point(265, 77);
                    ddong.Size = new Size(80, 77);
                    PlayPanel.Controls.Add(ddong);
                    ddong.BringToFront();
                    Delay(2000);
                    PlayPanel.Controls.Remove(ddong);
                    return;
                }
            }

            for (int k = 1; k <= 12; k++)
            {
                //유저가 k월 카드를 냈는데, k월의 카운트가 2일때 -> 먹어야함
                if (cs.AllCard[cs.ComputerCard[cs.put_comCard]].month == k && cs.month_count[k - 1] == 2)
                {
                    //k월인 2장의 카드의 값과 이미지를 캐치해야함

                    for (int j = 0; j < 2; j++)
                    {   // 2장을 지워야하기때문에 같은로직 2번 돌림
                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                //해당 인덱스 저장
                                cs.ComGetCard.Add(cs.CardOnBoard[p - 1].index);

                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_com_x + cs.interval * cs.pc_p_count, cs.P_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.pc_p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.pc_p_count += 2;

                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_com_x + cs.interval * cs.pc_d_count, cs.D_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_d_count++;
                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_com_x + cs.interval * cs.pc_a_count, cs.A_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_a_count++;
                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_com_x + cs.interval * cs.pc_g_count, cs.G_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_g_count++;
                                    cs.month_count[k - 1]--;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                        }//for i
                    }//for j
                }
                else if (cs.AllCard[cs.ComputerCard[cs.put_comCard]].month == k && cs.month_count[k - 1] == 3 && cs.ddong == false)
                {
                    //필드에 2장 깔려있는 카드의 월을 냈을때, 내가 낸것과 깔려있었던 둘중 하나의 것을 가져옴

                    for (int j = 0; j < 2; j++)
                    {   // 2장을 지워야하기때문에 같은로직 2번 돌림

                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                //해당 인덱스 저장
                                cs.ComGetCard.Add(cs.CardOnBoard[p - 1].index);


                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_com_x + cs.interval * cs.pc_p_count, cs.P_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.pc_p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.pc_p_count += 2;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_com_x + cs.interval * cs.pc_d_count, cs.D_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_d_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_com_x + cs.interval * cs.pc_a_count, cs.A_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_a_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_com_x + cs.interval * cs.pc_g_count, cs.G_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_g_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                            if (cs.month_count[k - 1] == 1)
                                return;
                        }//for i

                    }//for j

                }
                else if (cs.AllCard[cs.ComputerCard[cs.put_comCard]].month == k && cs.month_count[k - 1] == 4)
                {
                    //필드에 3장 깔려있는 카드의 월을 냈을때, 4장 다 가져옴

                    for (int j = 0; j < 4; j++)
                    {   // 4장을 지워야하기때문에 같은로직 4번 돌림

                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                //해당 인덱스 저장
                                cs.ComGetCard.Add(cs.CardOnBoard[p - 1].index);


                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_com_x + cs.interval * cs.pc_p_count, cs.P_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.pc_p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.pc_p_count += 2;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_com_x + cs.interval * cs.pc_d_count, cs.D_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_d_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_com_x + cs.interval * cs.pc_a_count, cs.A_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_a_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_com_x + cs.interval * cs.pc_g_count, cs.G_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_g_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                        }//for i

                    }//for j

                }//if-else-if
            }//for k
        }

        //컴퓨터가 뽑은 카드랑 겹쳤던 카드들을 컴퓨터의 스코어패널에 놓는 함수
        public void setDrawComCardOnScorePanel()
        {
            //유저가 똥쌌으면 아무것도 하지않고 넘김
            if (cs.ddong == true)
            {
                cs.ddong = false;
                return;
            }

            for (int k = 1; k <= 12; k++)
            {
                //유저가 k월 카드를 냈는데, k월의 카운트가 2일때 -> 먹어야함
                if (cs.AllCard[cs.CardDummy[cs.remain_Card + 1]].month == k && cs.month_count[k - 1] == 2)
                {
                    //k월인 2장의 카드의 값과 이미지를 캐치해야함

                    for (int j = 0; j < 2; j++)
                    {   // 2장을 지워야하기때문에 같은로직 2번 돌림
                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                //해당 인덱스 저장
                                cs.ComGetCard.Add(cs.CardOnBoard[p - 1].index);


                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_com_x + cs.interval * cs.pc_p_count, cs.P_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.pc_p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.pc_p_count += 2;
                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_com_x + cs.interval * cs.pc_d_count, cs.D_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_d_count++;
                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_com_x + cs.interval * cs.pc_a_count, cs.A_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_a_count++;
                                    cs.month_count[k - 1]--;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_com_x + cs.interval * cs.pc_g_count, cs.G_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_g_count++;
                                    cs.month_count[k - 1]--;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                        }//for i
                    }//for j
                }
                else if (cs.AllCard[cs.CardDummy[cs.remain_Card + 1]].month == k && cs.month_count[k - 1] == 3 && cs.ddong == false)
                {
                    //필드에 2장 깔려있는 카드의 월을 냈을때, 내가 낸것과 깔려있었던 둘중 하나의 것을 가져옴

                    for (int j = 0; j < 2; j++)
                    {   // 2장을 지워야하기때문에 같은로직 2번 돌림

                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                //해당 인덱스 저장
                                cs.ComGetCard.Add(cs.CardOnBoard[p - 1].index);


                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_com_x + cs.interval * cs.pc_p_count, cs.P_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.pc_p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.pc_p_count += 2;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_com_x + cs.interval * cs.pc_d_count, cs.D_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_d_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_com_x + cs.interval * cs.pc_a_count, cs.A_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_a_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_com_x + cs.interval * cs.pc_g_count, cs.G_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_g_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                            if (cs.month_count[k - 1] == 1)
                                return;
                        }//for i
                    }//for j

                }
                else if (cs.AllCard[cs.CardDummy[cs.remain_Card + 1]].month == k && cs.month_count[k - 1] == 4)
                {
                    //뽑은 카드가 필드에 3장 깔려있는 카드의 월을 가지는 카드일때, 4장 다가져옴

                    for (int j = 0; j < 4; j++)
                    {   // 4장을 지워야하기때문에 같은로직 4번 돌림

                        for (int p = cs.cardNum; p >= 1; p--)
                        {
                            if (cs.CardOnBoard[p - 1].month == k)
                            {
                                //해당 픽쳐박스 삭제
                                PlayPanel.Controls.Remove(cs.cardOnField[p - 1]);
                                cs.cardNum--;
                                if (cs.CardOnBoard[p - 1].value == 1 || cs.CardOnBoard[p - 1].value == 2)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.P_com_x + cs.interval * cs.pc_p_count, cs.P_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    if (cs.CardOnBoard[p - 1].value == 1)   //그냥 피면 피 count +1
                                        cs.pc_p_count++;
                                    else if (cs.CardOnBoard[p - 1].value == 2)  //쌍피면 피 count +2
                                        cs.pc_p_count += 2;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 4)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.D_com_x + cs.interval * cs.pc_d_count, cs.D_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_d_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 8)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.A_com_x + cs.interval * cs.pc_a_count, cs.A_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_a_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                else if (cs.CardOnBoard[p - 1].value == 16)
                                {
                                    cs.cardOnField[p - 1].Location = new Point(cs.G_com_x + cs.interval * cs.pc_g_count, cs.G_com_y);
                                    computerScorePanel.Controls.Add(cs.cardOnField[p - 1]);
                                    cs.cardOnField[p - 1].BringToFront();
                                    cs.pc_g_count++;
                                    cs.month_count[k - 1] -= 1;
                                }
                                //먹어서 사라진 카드들 리스트에서 삭제
                                cs.CardOnBoard.RemoveAt(p - 1);
                                cs.cardOnField.RemoveAt(p - 1);
                            }
                        }//for i
                    }//for j

                }//if-else-if
            }//for k

            


        }

        //----------------------------------유저가 카드 선택 시, 발생 이벤트------------------------------------------------------

        private void userCard1_Click(object sender, EventArgs e)
        {
            userCard1.Enabled = false;
            userCard1.Visible = false;

            //카드 정보를 playpanel에 전달하는 과정 필요
            //유저의 첫번째 패와 같은 이미지를 가지는 임의의 카드를 생성
            PictureBox spendCard = new PictureBox();
            spendCard.Image = cs.AllCard[cs.UserCard[0]].image;
            compare_userCard(spendCard, 0);
            spendCard.Size = new Size(40, 63);
            PlayPanel.Controls.Add(spendCard);
            spendCard.BringToFront();
            cs.cardOnField.Add(spendCard);
            cs.CardOnBoard.Add(cs.AllCard[cs.UserCard[0]]);
            cs.cardNum++;

            Delay(300);
            draw_Card();
            Delay(300);
            setUserCardOnScorePanel(spendCard, 0);
            Delay(300);
            setDrawCardOnScorePanel();
            Delay(300);

            checkWin();
            cs.turn = false;    //유저가 할 행동을 다 했으므로 상대방으로 턴을 넘김

            com_PutCardDown();
        }

        private void userCard2_Click(object sender, EventArgs e)
        {
            userCard2.Enabled = false;
            userCard2.Visible = false;

            PictureBox spendCard = new PictureBox();
            spendCard.Image = cs.AllCard[cs.UserCard[1]].image;
            compare_userCard(spendCard, 1);
            spendCard.Size = new Size(40, 63);
            PlayPanel.Controls.Add(spendCard);
            spendCard.BringToFront();
            cs.cardOnField.Add(spendCard);
            cs.CardOnBoard.Add(cs.AllCard[cs.UserCard[1]]);
            cs.cardNum++;

            Delay(300);
            draw_Card();
            Delay(300);
            setUserCardOnScorePanel(spendCard, 1);
            Delay(300);
            setDrawCardOnScorePanel();
            Delay(300);

            checkWin();
            cs.turn = false;    //유저가 할 행동을 다 했으므로 상대방으로 턴을 넘김

            com_PutCardDown();
        }

        private void userCard3_Click(object sender, EventArgs e)
        {
            userCard3.Enabled = false;
            userCard3.Visible = false;

            PictureBox spendCard = new PictureBox();
            spendCard.Image = cs.AllCard[cs.UserCard[2]].image;
            compare_userCard(spendCard, 2);
            spendCard.Size = new Size(40, 63);
            PlayPanel.Controls.Add(spendCard);
            spendCard.BringToFront();
            cs.cardOnField.Add(spendCard);
            cs.CardOnBoard.Add(cs.AllCard[cs.UserCard[2]]);
            cs.cardNum++;

            Delay(300);
            draw_Card();
            Delay(300);
            setUserCardOnScorePanel(spendCard, 2);
            Delay(300);
            setDrawCardOnScorePanel();
            Delay(300);

            cs.turn = false;    //유저가 할 행동을 다 했으므로 상대방으로 턴을 넘김
            checkWin();
            com_PutCardDown();
        }

        private void userCard4_Click(object sender, EventArgs e)
        {
            userCard4.Enabled = false;
            userCard4.Visible = false;

            PictureBox spendCard = new PictureBox();
            spendCard.Image = cs.AllCard[cs.UserCard[3]].image;
            compare_userCard(spendCard, 3);
            spendCard.Size = new Size(40, 63);
            PlayPanel.Controls.Add(spendCard);
            spendCard.BringToFront();
            cs.cardOnField.Add(spendCard);
            cs.CardOnBoard.Add(cs.AllCard[cs.UserCard[3]]);
            cs.cardNum++;

            Delay(300);
            draw_Card();
            Delay(300);
            setUserCardOnScorePanel(spendCard, 3);
            Delay(300);
            setDrawCardOnScorePanel();
            Delay(300);

            checkWin(); cs.turn = false;    //유저가 할 행동을 다 했으므로 상대방으로 턴을 넘김

            com_PutCardDown();
        }

        private void userCard5_Click(object sender, EventArgs e)
        {
            userCard5.Enabled = false;
            userCard5.Visible = false;

            PictureBox spendCard = new PictureBox();
            spendCard.Image = cs.AllCard[cs.UserCard[4]].image;
            compare_userCard(spendCard, 4);
            spendCard.Size = new Size(40, 63);
            PlayPanel.Controls.Add(spendCard);
            spendCard.BringToFront();
            cs.cardOnField.Add(spendCard);
            cs.CardOnBoard.Add(cs.AllCard[cs.UserCard[4]]);
            cs.cardNum++;

            Delay(300);
            draw_Card();
            Delay(300);
            setUserCardOnScorePanel(spendCard, 4);
            Delay(300);
            setDrawCardOnScorePanel();
            Delay(300);

            checkWin(); cs.turn = false;    //유저가 할 행동을 다 했으므로 상대방으로 턴을 넘김

            com_PutCardDown();
        }

        private void userCard6_Click(object sender, EventArgs e)
        {
            userCard6.Enabled = false;
            userCard6.Visible = false;

            PictureBox spendCard = new PictureBox();
            spendCard.Image = cs.AllCard[cs.UserCard[5]].image;
            compare_userCard(spendCard, 5);
            spendCard.Size = new Size(40, 63);
            PlayPanel.Controls.Add(spendCard);
            spendCard.BringToFront();
            cs.cardOnField.Add(spendCard);
            cs.CardOnBoard.Add(cs.AllCard[cs.UserCard[5]]);
            cs.cardNum++;

            Delay(300);
            draw_Card();
            Delay(300);
            setUserCardOnScorePanel(spendCard, 5);
            Delay(300);
            setDrawCardOnScorePanel();
            Delay(300);

            checkWin(); cs.turn = false;    //유저가 할 행동을 다 했으므로 상대방으로 턴을 넘김

            com_PutCardDown();
        }

        private void userCard7_Click(object sender, EventArgs e)
        {
            userCard7.Enabled = false;
            userCard7.Visible = false;

            PictureBox spendCard = new PictureBox();
            spendCard.Image = cs.AllCard[cs.UserCard[6]].image;
            compare_userCard(spendCard, 6);
            spendCard.Size = new Size(40, 63);
            PlayPanel.Controls.Add(spendCard);
            spendCard.BringToFront();
            cs.cardOnField.Add(spendCard);
            cs.CardOnBoard.Add(cs.AllCard[cs.UserCard[6]]);
            cs.cardNum++;

            Delay(300);
            draw_Card();
            Delay(300);
            setUserCardOnScorePanel(spendCard, 6);
            Delay(300);
            setDrawCardOnScorePanel();
            Delay(300);

            checkWin(); cs.turn = false;    //유저가 할 행동을 다 했으므로 상대방으로 턴을 넘김

            com_PutCardDown();
        }

        private void userCard8_Click(object sender, EventArgs e)
        {
            userCard8.Enabled = false;
            userCard8.Visible = false;

            PictureBox spendCard = new PictureBox();
            spendCard.Image = cs.AllCard[cs.UserCard[7]].image;
            compare_userCard(spendCard, 7);
            spendCard.Size = new Size(40, 63);
            PlayPanel.Controls.Add(spendCard);
            spendCard.BringToFront();
            cs.cardOnField.Add(spendCard);
            cs.CardOnBoard.Add(cs.AllCard[cs.UserCard[7]]);
            cs.cardNum++;

            Delay(300);
            draw_Card();
            Delay(300);
            setUserCardOnScorePanel(spendCard, 7);
            Delay(300);
            setDrawCardOnScorePanel();
            Delay(300);

            checkWin(); cs.turn = false;    //유저가 할 행동을 다 했으므로 상대방으로 턴을 넘김

            com_PutCardDown();
        }

        private void userCard9_Click(object sender, EventArgs e)
        {
            userCard9.Enabled = false;
            userCard9.Visible = false;

            PictureBox spendCard = new PictureBox();
            spendCard.Image = cs.AllCard[cs.UserCard[8]].image;
            compare_userCard(spendCard, 8);
            spendCard.Size = new Size(40, 63);
            PlayPanel.Controls.Add(spendCard);
            spendCard.BringToFront();
            cs.cardOnField.Add(spendCard);
            cs.CardOnBoard.Add(cs.AllCard[cs.UserCard[8]]);
            cs.cardNum++;

            Delay(300);
            draw_Card();
            Delay(300);
            setUserCardOnScorePanel(spendCard, 8);
            Delay(300);
            setDrawCardOnScorePanel();
            Delay(300);

            checkWin(); cs.turn = false;    //유저가 할 행동을 다 했으므로 상대방으로 턴을 넘김

            com_PutCardDown();
        }

        private void userCard10_Click(object sender, EventArgs e)
        {
            userCard10.Enabled = false;
            userCard10.Visible = false;

            PictureBox spendCard = new PictureBox();
            spendCard.Image = cs.AllCard[cs.UserCard[9]].image;
            compare_userCard(spendCard, 9);
            spendCard.Size = new Size(40, 63);
            PlayPanel.Controls.Add(spendCard);
            spendCard.BringToFront();
            cs.cardOnField.Add(spendCard);
            cs.CardOnBoard.Add(cs.AllCard[cs.UserCard[9]]);
            cs.cardNum++;

            Delay(300);
            draw_Card();
            Delay(300);
            setUserCardOnScorePanel(spendCard, 9);
            Delay(300);
            setDrawCardOnScorePanel();
            Delay(300);

            checkWin(); cs.turn = false;    //유저가 할 행동을 다 했으므로 상대방으로 턴을 넘김

            com_PutCardDown();
        }


        //-------------------------------점수계산--------------------

        //피박등 적용전 점수
        private int countCard(int p_count, int a_count, int d_count, int g_count)//피,동물,띠,광
        {
            //초기화
            int score = 0;

            //피 10개 1점 10개 이상시 
            if (p_count >= 10)
            {
                score += p_count - 9;
            }

            //띠&동물 5개 1점 /  이상시 장당1점 추가
            if (a_count >= 5)
            {
                score += a_count - 4;
            }

            //동물과 같음
            if (d_count >= 5)
            {
                score += d_count - 4;
            }

            //광 3개이상일때부터 개당 1점
            if (g_count >= 3)
            {
                score += g_count;
                checkGwang(g_count);
            }

            return score;
        }
        
        //3광 4광 5광 이미지 출력 함수
        public void checkGwang(int count)
        {
            switch (count)
            {
                case 3:
                    if (cs.gwang_trigger[0] == false) {
                        PictureBox g3 = new PictureBox();
                        g3.BackgroundImage = Properties.Resources.main2;
                        g3.Image = Properties.Resources._3gwang;
                        g3.Location = new Point(200, 50);
                        g3.Size = new Size(231, 180);
                        PlayPanel.Controls.Add(g3);
                        g3.BringToFront();
                        Delay(2000);
                        PlayPanel.Controls.Remove(g3);
                        cs.gwang_trigger[0] = true;
                    }
                    break;
                case 4:
                    if(cs.gwang_trigger[1] == false) {
                        PictureBox g4 = new PictureBox();
                        g4.BackgroundImage = Properties.Resources.main2;
                        g4.Image = Properties.Resources._4gwang;
                        g4.Location = new Point(200, 50);
                        g4.Size = new Size(231, 180);
                        PlayPanel.Controls.Add(g4);
                        g4.BringToFront();
                        Delay(2000);
                        PlayPanel.Controls.Remove(g4);
                        cs.gwang_trigger[1] = true;
                    }
                    break;
                case 5:
                    if (cs.gwang_trigger[2] == false)
                    {
                        PictureBox g5 = new PictureBox();
                        g5.BackgroundImage = Properties.Resources.main2;
                        g5.Image = Properties.Resources._5gwang;
                        g5.Location = new Point(200, 50);
                        g5.Size = new Size(231, 180);
                        PlayPanel.Controls.Add(g5);
                        g5.BringToFront();
                        Delay(2000);
                        PlayPanel.Controls.Remove(g5);
                        cs.gwang_trigger[2] = true;
                    }
                    break;
            }
        }

        //고도리, 홍단, 청단, 초단에 따라 점수 추가
        int checkCondition(string condition, int score)
        {
            string _godori = "고도리";
            string _chodan = "초단";
            string _hongdan = "홍단";
            string _chungdan = "청단";

            if (condition.Equals(_godori))
            {
                score += 5;
                return score;
            }
            else if (condition.Equals(_chodan) || condition.Equals(_hongdan) || condition.Equals(_chungdan))
            {
                score += 3;
                return score;
            }

            return 0;
        }

        //유저 점수
        public int countUserScore()
        {
            //유저값 넣어서 넘겨줌
            int score = countCard(cs.p_count, cs.a_count, cs.d_count, cs.g_count);

            int godori_count = 0;
            int chodan_count = 0;
            int hongdan_count = 0;
            int chungdan_count = 0;

            MyDelegate showImage;

            showImage = new MyDelegate(checkCondition);

            //------------고도리 / 홍단 / 청단 / 초단 체크---------------
            foreach (var i in cs.UserGetCard)
            {
                //고도리
                if (i == 4 || i == 12 || i == 29)
                {
                    godori_count++;
                }

                //홍단
                if (i == 5 || i == 9 || i == 1)
                {
                    hongdan_count++;
                }

                //청단
                if (i == 21 || i == 33 || i == 37)
                {
                    chungdan_count++;
                }

                //초단
                if (i == 13 || i == 17 || i == 25)
                {
                    chodan_count++;
                }
            }

            //고도리 되면 점수+5
            if (godori_count >= 3)
            {
                if (cs.godori_trigger == false)
                {
                    PictureBox godori = new PictureBox();
                    godori.BackgroundImage = Properties.Resources.main2;
                    godori.Image = Properties.Resources.godori;
                    godori.Location = new Point(200, 50);
                    godori.Size = new Size(246, 173);
                    PlayPanel.Controls.Add(godori);
                    godori.BringToFront();
                    Delay(2000);
                    PlayPanel.Controls.Remove(godori);
                    cs.godori_trigger = true;
                }

                score += showImage("고도리", score);
            }

            //초단 되면 점수+3
            if (chodan_count >= 3)
            {
                if (cs.chodan_trigger == false)
                {
                    PictureBox chodan = new PictureBox();
                    chodan.BackgroundImage = Properties.Resources.main2;
                    chodan.Image = Properties.Resources.chodan;
                    chodan.Location = new Point(200, 50);
                    chodan.Size = new Size(246, 201);
                    PlayPanel.Controls.Add(chodan);
                    chodan.BringToFront();
                    Delay(2000);
                    PlayPanel.Controls.Remove(chodan);
                    cs.chodan_trigger = true;
                }
                score += showImage("초단", score);
            }

            //홍단되면 점수+3
            if (hongdan_count >= 3)
            {
                if (cs.hongdan_trigger == false)
                {
                    PictureBox hongdan = new PictureBox();
                    hongdan.BackgroundImage = Properties.Resources.main2;
                    hongdan.Image = Properties.Resources.hongdan;
                    hongdan.Location = new Point(200, 50);
                    hongdan.Size = new Size(246, 182);
                    PlayPanel.Controls.Add(hongdan);
                    hongdan.BringToFront();
                    Delay(2000);
                    PlayPanel.Controls.Remove(hongdan);
                    cs.hongdan_trigger = true;
                }
                score += showImage("홍단", score);
            }

            //청단되면 점수+3
            if (chungdan_count >= 3)
            {
                if (cs.chungdan_trigger == false)
                {
                    PictureBox chungdan = new PictureBox();
                    chungdan.BackgroundImage = Properties.Resources.main2;
                    chungdan.Image = Properties.Resources.chungdan;
                    chungdan.Location = new Point(200, 50);
                    chungdan.Size = new Size(246, 185);
                    PlayPanel.Controls.Add(chungdan);
                    chungdan.BringToFront();
                    Delay(2000);
                    PlayPanel.Controls.Remove(chungdan);
                    cs.chungdan_trigger = true;
                }
                score += showImage("청단", score);
            }

            //고 할때마다 1점씩 추가
            score += cs.user_go;

            //상대 피가 5장이하일때 피박
            //피박이면 이제까지 총점수의 두배
            //난거 아니면 계산안함
            if (cs.pc_p_count < 6 )//&& score >= 7)
            {
                if(score >= 7)
                    score *= 2;
            }

            return score;
        }

        //컴 점수
        public int countComScore()
        {
            int score = countCard(cs.pc_p_count, cs.pc_a_count, cs.pc_d_count, cs.pc_g_count);

            int godori_count = 0;
            int chodan_count = 0;
            int hongdan_count = 0;
            int chungdan_count = 0;

            MyDelegate showImage;
            showImage = new MyDelegate(checkCondition);
            //------------고도리 / 홍단 / 청단 / 초단 체크---------------
            foreach (var i in cs.ComGetCard)
            //for(int i=0;i<cs.ComGetCard.s)
            {
                //고도리
                if (i == 4 || i == 12 || i == 29)
                {
                    godori_count++;
                }

                //홍단
                if (i == 5 || i == 9 || i == 1)
                {
                    hongdan_count++;
                }

                //청단
                if (i == 21 || i == 33 || i == 37)
                {
                    chungdan_count++;
                }

                //초단
                if (i == 13 || i == 17 || i == 25)
                {
                    chodan_count++;
                }
            }

            //고도리 되면 점수+5
            if (godori_count >= 3)
            {
                if (cs.godori_trigger == false)
                {
                    PictureBox godori = new PictureBox();
                    godori.BackgroundImage = Properties.Resources.main2;
                    godori.Image = Properties.Resources.godori;
                    godori.Location = new Point(200, 50);
                    godori.Size = new Size(246, 173);
                    PlayPanel.Controls.Add(godori);
                    godori.BringToFront();
                    Delay(2000);
                    PlayPanel.Controls.Remove(godori);
                    cs.godori_trigger = true;
                }

                score += showImage("고도리", score);
            }


            //초단 되면 점수+3
            if (chodan_count >= 3)
            {
                if (cs.chodan_trigger == false)
                {
                    PictureBox chodan = new PictureBox();
                    chodan.BackgroundImage = Properties.Resources.main2;
                    chodan.Image = Properties.Resources.chodan;
                    chodan.Location = new Point(200, 50);
                    chodan.Size = new Size(246, 201);
                    PlayPanel.Controls.Add(chodan);
                    chodan.BringToFront();
                    Delay(2000);
                    PlayPanel.Controls.Remove(chodan);
                    cs.chodan_trigger = true;
                }
                score += showImage("초단", score);
            }


            //홍단되면 점수+3
            if (hongdan_count >= 3)
            {
                if (cs.hongdan_trigger == false)
                {
                    PictureBox hongdan = new PictureBox();
                    hongdan.BackgroundImage = Properties.Resources.main2;
                    hongdan.Image = Properties.Resources.hongdan;
                    hongdan.Location = new Point(200, 50);
                    hongdan.Size = new Size(246, 182);
                    PlayPanel.Controls.Add(hongdan);
                    hongdan.BringToFront();
                    Delay(2000);
                    PlayPanel.Controls.Remove(hongdan);
                    cs.hongdan_trigger = true;
                }
                score += showImage("홍단", score);
            }


            //청단되면 점수+3
            if (chungdan_count >= 3)
            {
                if (cs.chungdan_trigger == false)
                {
                    PictureBox chungdan = new PictureBox();
                    chungdan.BackgroundImage = Properties.Resources.main2;
                    chungdan.Image = Properties.Resources.chungdan;
                    chungdan.Location = new Point(200, 50);
                    chungdan.Size = new Size(246, 185);
                    PlayPanel.Controls.Add(chungdan);
                    chungdan.BringToFront();
                    Delay(2000);
                    PlayPanel.Controls.Remove(chungdan);
                    cs.chungdan_trigger = true;
                }
                score += showImage("청단", score);
            }

            //고 할때마다 1점씩 추가
            score += cs.com_go;

            //상대 피가 5장이하일때 피박
            //피박이면 이제까지 총점수의 두배
            //컴일땐 유저피상태와 비교
            if (cs.pc_p_count < 6)//&& score >= 7)
            {
                if (score >= 7)
                    score *= 2;
            }

            return score;
        }

        public void checkWin()
        {

            int UserCount = countCard(cs.p_count, cs.a_count, cs.d_count, cs.g_count);
            int ComCount = countCard(cs.pc_p_count, cs.pc_a_count, cs.pc_d_count, cs.pc_g_count);

            int userScore = countUserScore();
            int comScore = countComScore();


            ComPricelabel.Text = "Com : " + countComScore().ToString() +" 점";
            UserPricelabel.Text = "User : " + countUserScore().ToString() + " 점";
            //CardPricelabel.Text = "Card : " + countCard(cs.p_count, cs.a_count, cs.d_count, cs.g_count).ToString() + " 점";
            lblGo.Text = "G O : " + cs.user_go + " 고";
            lblComGo.Text = "G O : " + cs.com_go + " 고";

            //둘다 7점을 못넘으면 동점(나가리)
            if (comScore < 7 && userScore < 7 && cs.remain_Card < 0)
            {
                cs.whoWin = WHOWIN.DRAW;
            }

            //남은 카드가 없으면 종료
            if (cs.remain_Card < 0)
            {
                if (countUserScore() < countComScore())
                {
                    cs.whoWin = WHOWIN.COMWIN;
                    cs.winScore = countComScore();
                }
                else
                {
                    cs.whoWin = WHOWIN.USERWIN;
                    cs.winScore = countUserScore();
                }

                showStopImage();
                showScoreForm();
            }

            //유저가 7점 넘으면?
            else if (userScore >= 7&&cs.turn)
            {
                //컴퓨터가 고했으면 광박으로 유저승
                if (cs.com_go > 0)
                {
                    cs.whoWin = WHOWIN.USERWIN;
                    cs.winScore = comScore;
                    showStopImage();
                   showScoreForm();
                }

                //점수 추가획득이 있어야만 고 가능
                else if (cs.beforeUserScore < UserCount && cs.remain_Card>1)
                {
                    UserGoOrStop();
                }

            }

            //컴이 7점넘으면
            else if (comScore >= 7&& !cs.turn)
            {
                //유저가 고했으면 광박으로 컴승
                if (cs.user_go > 0)
                {
                    cs.whoWin = WHOWIN.COMWIN;
                    cs.winScore = countComScore();
                    showStopImage();
                   showScoreForm();
                }

                //점수증가가 있었고
                //컴퓨터 턴이고
                //플레이어의 점수가 5점 미만이면 고
                else if (cs.beforeComScore < ComCount && UserCount < 5 && !cs.turn)
                {
                    cs.com_go++;

                    showGoImage(cs.com_go);

                }

                //고안하면 컴승리
                else
                {
                    showStopImage();
                    cs.whoWin = WHOWIN.COMWIN;
                    cs.winScore = countComScore();
                    showStopImage();
                    showScoreForm();
                }

            }

            cs.beforeUserScore = countCard(cs.p_count, cs.a_count, cs.d_count, cs.g_count);
            cs.beforeComScore = countCard(cs.pc_p_count, cs.pc_a_count, cs.pc_d_count, cs.pc_g_count);
        }

        //유저한테 고할지 스톱할지 묻는함수
        public void UserGoOrStop()
        {
            Go_or_Stop GoOrStop = new Go_or_Stop();

            //GoOrStop.ShowDialog();
            //go함
            //유저턴이면
            //마지막턴엔 고안함

            int isGo = 0;//1이면 고 2면 스탑

            switch (GoOrStop.ShowDialog())
            {
                case System.Windows.Forms.DialogResult.OK:
                    isGo = 1;
                    break;
                case System.Windows.Forms.DialogResult.No:
                    isGo = 2;
                    break;
            }
                if (isGo ==1 && cs.turn)
            {
                cs.user_go++;
                showGoImage(cs.user_go);
                return;
            }

            //stop함
            else if (isGo ==2)
            {

                //고안하면 유저승리
                cs.whoWin = WHOWIN.USERWIN;
                cs.winScore = countUserScore();
                showStopImage();
                showScoreForm();
                return;
            }
            
        }

        public void showScoreForm()
        {
            int score = cs.winScore;
            int whowin = 0; // user 승리시 1, computer 승리시 2, draw일 시 3
            int chongtong = cs.whoChongtong; //User 총통 시 1, Com 총통 시 2, 총통 X 시 0
            if (cs.whoWin == WHOWIN.USERWIN)
                whowin = 1;
            else if (cs.whoWin == WHOWIN.COMWIN)
                whowin = 2;
            else if (cs.whoWin == WHOWIN.DRAW)
                whowin = 3;
            ScoreForm sf = new ScoreForm(whowin, score, chongtong);
            sf.Owner = this;
            this.Hide();
            sf.ShowDialog();
            //this.Close();
        }


        //고할때 이미지 띄우기
        public void showGoImage(int n)
        {

            PictureBox gogo = new PictureBox();
            gogo.BackgroundImage = Properties.Resources.main2;

            switch (n)
            {
                case 1:
                    gogo.Image = Properties.Resources._1go;
                    break;
                case 2:
                    gogo.Image = Properties.Resources._2go;
                    break;
                case 3:
                    gogo.Image = Properties.Resources._3go;
                    break;
                case 4:
                    gogo.Image = Properties.Resources._4go;
                    break;
                case 5:
                    gogo.Image = Properties.Resources._5go;
                    break;
                case 6:
                    gogo.Image = Properties.Resources._6go;
                    break;
            }

            gogo.Location = new Point(220, 77);
            gogo.Size = new Size(178, 88);
            PlayPanel.Controls.Add(gogo);
            gogo.BringToFront();
            Delay(2000);
            PlayPanel.Controls.Remove(gogo);
            return;
        }


        //스톱할때 이미지 띄우기
        public void showStopImage()
        {
            PictureBox stop = new PictureBox();
            stop.BackgroundImage = Properties.Resources.main2;

            stop.Image = Properties.Resources.stop_1;

            stop.Location = new Point(200, 77);
            stop.Size = new Size(214, 82);
            PlayPanel.Controls.Add(stop);
            stop.BringToFront();
            Delay(2000);
            PlayPanel.Controls.Remove(stop);
            return;
        }
    }

}