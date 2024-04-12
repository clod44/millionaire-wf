using System.Windows.Forms;

namespace Millionaire
{
    public partial class Form1 : Form
    {
        Random r = new Random();

        int questionsPerStage = 10;
        int[] rewards = { 100, 1000, 2000, 3000, 5000, 7500, 15000, 30000, 60000, 125000, 500000, 1000000 };
        string[,,] questionPool;
        //player 
        int playerStage = 0;
        int playerStageQuestionId = 0; //necessary for re-updating choices in case of a joker usage that effects them.
        int guaranteedBalance = 0;
        //phone, ask viewers, 50 50, double
        bool[] availableJokers = { true, true, true, true };

        bool doubleJoker = false;

        /*
        {
            //stage 1 questions
            {
                //question example 1
                {
                    "question text here", 
                    "choice A", 
                    "choice B",
                    "choice C", 
                    "choice D",
                    "correct choice index(0,3)"
                }
                //question example 2
                ...
            }
            //stage 2 questions
            ...
        } 
        */
        public Form1()
        {
            InitializeComponent();

            switchToTheTab(0);
            generatePlaceHolderQuestions();

        }


        private void generatePlaceHolderQuestions()
        {
            questionPool = new string[12, questionsPerStage, 6];
            for (int stage = 0; stage < 12; stage++)
            {
                for (int question = 0; question < questionsPerStage; question++)
                {
                    int correctAnswerIndex = r.Next(4);
                    questionPool[stage, question, 0] = $"This is stage {stage + 1}/12 and this is question{question} out of other {questionsPerStage} questions. answer is choice {correctAnswerIndex + 1}";
                    questionPool[stage, question, 1] = "Choice 1";
                    questionPool[stage, question, 2] = "Choice 2";
                    questionPool[stage, question, 3] = "Choice 3";
                    questionPool[stage, question, 4] = "Choice 4";
                    questionPool[stage, question, 5] = "" + correctAnswerIndex;
                }
            }
        }

        private void switchToTheTab(int tabId)
        {
            /*
             *   0 - menu
             *   1 - gameplay
             *   2 - credits
             * 
             * */
            if (tabId > tabControl_Game.TabCount - 1)
            {
                MessageBox.Show("Invalid tabId: " + tabId + " out of " + tabControl_Game.TabCount);
                return;
            }
            for (int i = 0; i < tabControl_Game.TabCount; i++)
            {
                tabControl_Game.TabPages[i].Enabled = false;
            }
            tabControl_Game.TabPages[tabId].Enabled = true;
            tabControl_Game.SelectedIndex = tabId;

        }

        private void resetGameProgress()
        {
            playerStage = 0;
            playerStageQuestionId = r.Next(questionsPerStage);
            guaranteedBalance = 0;

            availableJokers[0] = true;
            availableJokers[1] = true;
            availableJokers[2] = true;
            availableJokers[3] = true;
            updateJokerButtons();

            updateRewardsTable();
            showQuestion();
        }
        private void showQuestion()
        {
            //extract the selected question from pool to work with it easier
            string[] currentQuestion = new string[6];
            for (int i = 0; i < 6; i++) currentQuestion[i] = questionPool[playerStage, playerStageQuestionId, i];


            label_QuestionText.Text = currentQuestion[0];
            button_ChoiceA.Text = "A ) " + currentQuestion[1];
            button_ChoiceA.Enabled = true;
            button_ChoiceB.Text = "B ) " + currentQuestion[2];
            button_ChoiceB.Enabled = true;
            button_ChoiceC.Text = "C ) " + currentQuestion[3];
            button_ChoiceC.Enabled = true;
            button_ChoiceD.Text = "D ) " + currentQuestion[4];
            button_ChoiceD.Enabled = true;
        }

        private void nextQuestion() //this function only triggers when the answer is correct
        {
            playerStage = Math.Min(playerStage + 1, 11);    //for protection
            playerStageQuestionId = r.Next(questionsPerStage);
            showQuestion();

        }


        private bool createPopupYN(string caption, string message)
        {
            return (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
        }
        private void createPopupOK(string caption, string message)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        //prevent tab switching via clicking to the tabs. tabs which are ".Enabled=false;" will be locked
        //shamelessly stolen from internet
        private void tabControl_Game_Selecting(object sender, TabControlCancelEventArgs e)
        {
            e.Cancel = !((Control)e.TabPage).Enabled;
        }

        private void button_StartTheGame_Click(object sender, EventArgs e)
        {
            resetGameProgress();
            switchToTheTab(1);
        }

        private void button_Credits_Click(object sender, EventArgs e)
        {
            switchToTheTab(2);
        }

        private void button_Quit_Click(object sender, EventArgs e)
        {
            bool confirmation = createPopupYN("EXIT", "Are you sure you want to exit?");
            if (confirmation) Application.Exit();
        }

        private void button_GoBackToMenu_Click(object sender, EventArgs e)
        {
            switchToTheTab(0);
        }


        private void button_ContinueGame_Click(object sender, EventArgs e)
        {
            switchToTheTab(1);
            button_ContinueGame.Enabled = false; //to make sure. returning back normally will enable the button again anyways 
        }

        private void button_ReturnToMenu_Click(object sender, EventArgs e)
        {
            bool confirmation = createPopupYN("Return to Menu", "Are you sure you want to go back to menu?\n(your progress will be saved)");
            if (confirmation)
            {
                switchToTheTab(0);
                button_ContinueGame.Enabled = true;
            }
        }

        private void button_ChoiceA_Click(object sender, EventArgs e)
        {
            answeringQuestion(0);
        }

        private void button_ChoiceB_Click(object sender, EventArgs e)
        {
            answeringQuestion(1);
        }

        private void button_ChoiceC_Click(object sender, EventArgs e)
        {
            answeringQuestion(2);
        }

        private void button_ChoiceD_Click(object sender, EventArgs e)
        {
            answeringQuestion(3);
        }

        private void colorAChoice(int choiceIndex, Color c)
        {
            switch (choiceIndex)
            {
                case 0:
                    button_ChoiceA.BackColor = c;
                    break;
                case 1:
                    button_ChoiceB.BackColor = c;
                    break;
                case 2:
                    button_ChoiceC.BackColor = c;
                    break;
                case 3:
                    button_ChoiceD.BackColor = c;
                    break;
            }
        }
        private void answeringQuestion(int choiceIndex)
        {
            //color up the selected choice
            colorAChoice(choiceIndex, Color.Gold);

            string[] letters = { "A", "B", "C", "D" };
            bool confirmation = createPopupYN("Are You Sure?", "You are answering with the choice " + letters[choiceIndex]);
            if (confirmation)
            {
                bool isCorrectAnswer = (Convert.ToInt32(questionPool[playerStage, playerStageQuestionId, 5]) == choiceIndex);
                //color the thing correct
                if (isCorrectAnswer)
                {
                    //some special stages where you stop risking the money
                    if (playerStage == 1 || playerStage == 6 || playerStage == 11) guaranteedBalance = rewards[playerStage];

                    colorAChoice(choiceIndex, Color.Green);
                    createPopupOK("Correct Answer!", "wow amazing great bruh");

                    //this was the last question
                    if (playerStage == rewards.Length - 1)
                    {
                        createPopupOK("You Won " + rewards[rewards.Length - 1] + " TL", "congrats. go home now");
                        resetGameProgress();
                        switchToTheTab(0);
                    }
                    else
                    {
                        nextQuestion();
                        updateRewardsTable();
                    }
                }
                else
                {
                    colorAChoice(choiceIndex, Color.DarkRed);
                    if (doubleJoker) createPopupOK("Wrong answer! double answer saved you", "try again");
                    else
                    {
                        colorAChoice(Convert.ToInt32(questionPool[playerStage, playerStageQuestionId, 5]), Color.Green);
                        createPopupOK("Wrong answer!", "you lost. this is how much you earned:\n" + guaranteedBalance + "TL");
                        switchToTheTab(0);
                        resetGameProgress();
                    }
                }
                doubleJoker = false;
            }

            //turn back to normal colors
            Color c = new Color();
            c = Color.FromArgb(0, 0, 64);
            button_ChoiceA.BackColor = c;
            button_ChoiceB.BackColor = c;
            button_ChoiceC.BackColor = c;
            button_ChoiceD.BackColor = c;                     

        }

        //visuals only
        private void updateRewardsTable()
        {
            for (int i = 1; i <= 12; i++)
            {
                CheckBox checkBox = (CheckBox)Controls.Find("checkBox_Rewards" + i, true)[0];
                checkBox.Checked = i <= playerStage;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (playerStage == 0)
            {
                createPopupOK("are you sure?", "you didnt complete any stages to begin leaving with half. you can just return to menu if you want.");
            }
            else
            {
                int tempBalance = Convert.ToInt32(rewards[playerStage - 1] * 0.5);
                bool confirmation = createPopupYN("Are you sure?", $"Take {tempBalance}TL and leave?");
                if (confirmation)
                {
                    createPopupOK("Congrats", $"you got {tempBalance}TL and left home.");
                    switchToTheTab(0);
                    resetGameProgress();
                }
            }
        }

        private void updateJokerButtons()
        {
            button_Joker1.Enabled = availableJokers[0];
            button_Joker2.Enabled = availableJokers[1];
            button_Joker3.Enabled = availableJokers[2];
            button_Joker4.Enabled = availableJokers[3];
        }
        private void button_Joker1_Click(object sender, EventArgs e)
        {
            bool confirmation = createPopupYN("use Phone Joker?", "You will call your mom and she will give you her guess.");
            if (confirmation)
            {
                availableJokers[0] = false;
                updateJokerButtons();

                //mom is right
                int momsAnswer = Convert.ToInt32(questionPool[playerStage, playerStageQuestionId, 5]);
                string[] letters = { "A", "B", "C", "D" };

                if (r.Next(100) > 70)
                {
                    //mom is gonna say something random
                    List<int> possibleChoicesList = new List<int>();
                    if (button_ChoiceA.Enabled) possibleChoicesList.Add(0);
                    if (button_ChoiceB.Enabled) possibleChoicesList.Add(1);
                    if (button_ChoiceC.Enabled) possibleChoicesList.Add(2);
                    if (button_ChoiceD.Enabled) possibleChoicesList.Add(3);
                    int[] possibleChoices = possibleChoicesList.ToArray();
                    momsAnswer = possibleChoices[r.Next(possibleChoices.Length)];
                }
                createPopupOK("Phone Joker", "Your Mother's answer is " + letters[momsAnswer]);
            }
        }

        private void button_Joker2_Click(object sender, EventArgs e)
        {
            bool confirmation = createPopupYN("Ask Viewers?", "the viewers gonna give their answer to you.");
            if (confirmation)
            {
                availableJokers[1] = false;
                updateJokerButtons();

                int correctAnswer = Convert.ToInt32(questionPool[playerStage, playerStageQuestionId, 5]);
                int p0 = (button_ChoiceA.Enabled) ? r.Next(101) : 0;
                int p1 = (button_ChoiceB.Enabled) ? r.Next(101 - p0) : 0;
                int p2 = (button_ChoiceC.Enabled) ? r.Next(101 - p0 - p1) : 0;
                int p3 = (button_ChoiceD.Enabled) ? 101 - p0 - p1 - p2 : 0;
                int[] percentages = { p0, p1, p2, p3 };
                int highestP = -1;
                int highestPIndex = 0;
                for (int i = 0; i < 4; i++)                       
                {
                    if (percentages[i] > highestP)
                    {
                        highestP = percentages[i];
                        highestPIndex = i;
                    }
                }
                int temp = percentages[correctAnswer];
                percentages[correctAnswer] = highestP;
                percentages[highestPIndex] = temp;

                createPopupOK("Viewer results", "A: " + percentages[0] + "\nB: " + percentages[1] + "\nC: " + percentages[2] + "\nD: " + percentages[3]);

            }
        }

        private void button_Joker3_Click(object sender, EventArgs e)
        {
            bool confirmation = createPopupYN("50% joker?", "half of the choices will be removed.");
            if (confirmation)
            {
                availableJokers[2] = false;
                updateJokerButtons();

                int correctAnswer = Convert.ToInt32(questionPool[playerStage, playerStageQuestionId, 5]);
                int hiddenIndexCount = 0;
                int[] hiddenIndexes = { -1, -1 };
                for (int i = 0; i < 50; i++)
                {
                    int hideIndex = r.Next(4);
                    if (hideIndex == correctAnswer || hideIndex == hiddenIndexes[0] || hideIndex == hiddenIndexes[1]) continue;
                    hiddenIndexes[hiddenIndexCount] = hideIndex;
                    hiddenIndexCount++;
                    if (hiddenIndexCount == 2) break;
                }

                //hide them
                for (int i = 0; i < 2; i++)
                {
                    switch (hiddenIndexes[i])
                    {
                        case 0:
                            button_ChoiceA.Text = "A ) ";
                            button_ChoiceA.Enabled = false;
                            break;
                        case 1:
                            button_ChoiceB.Text = "B ) ";
                            button_ChoiceB.Enabled = false;
                            break;
                        case 2:
                            button_ChoiceC.Text = "C ) ";
                            button_ChoiceC.Enabled = false;
                            break;
                        case 3:
                            button_ChoiceD.Text = "D ) ";
                            button_ChoiceD.Enabled = false;
                            break;
                    }
                }
            }
        }

        private void button_Joker4_Click(object sender, EventArgs e)
        {
            bool confirmation = createPopupYN("use Double Answer joker?", "you will be able to answer twice.");
            if (confirmation)
            {
                availableJokers[3] = false;
                updateJokerButtons();       

                doubleJoker = true;
            }
        }
    }
}