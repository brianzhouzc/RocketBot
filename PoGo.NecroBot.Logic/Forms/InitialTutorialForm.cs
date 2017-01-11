using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Protobuf.Collections;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Enums;
using POGOProtos.Data.Player;
using POGOProtos.Networking.Responses;
using PoGo.NecroBot.Logic.Event;

namespace PoGo.NecroBot.Logic.Forms
{
    public partial class InitialTutorialForm : Form
    {
        private ISession session;
        private RepeatedField<TutorialState> tutState;
        CheckTosState state;
        public InitialTutorialForm()
        {
            InitializeComponent();
        }

        public InitialTutorialForm(CheckTosState s, RepeatedField<TutorialState> tutState, ISession session)
        {
            InitializeComponent();
            this.state = s;
            this.tutState = tutState;
            this.session = session;



        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void wizardPage4_Initialize(object sender, AeroWizard.WizardPageInitEventArgs e)
        {
            Task.Run(() =>
            {
                Gender gen = rdoMale.Checked ? Gender.Male : Gender.Female;

                var avatarRes = session.Client.Player.SetAvatar(new PlayerAvatar()
                {
                    Backpack = 0,
                    Eyes = 0,
                    Gender = gen,
                    Hair = 0,
                    Hat = 0,
                    Pants = 0,
                    Shirt = 0,
                    Shoes = 0,
                    Skin = 0
                }).Result;

                if (avatarRes.Status == SetAvatarResponse.Types.Status.AvatarAlreadySet ||
                    avatarRes.Status == SetAvatarResponse.Types.Status.Success)
                {
                    session.Client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>()
                    {
                        TutorialState.AvatarSelection
                    }).Wait();
                    session.EventDispatcher.Send(new NoticeEvent()
                    {
                        Message = $"Selected your avatar, now you are {gen}!"
                    });
                }
            }).Wait();
            wizardControl1.NextPage();
        }

        private void wizardPage3_Initialize(object sender, AeroWizard.WizardPageInitEventArgs e)
        {
            PokemonId firstPoke = rdoBulbasaur.Checked ? PokemonId.Bulbasaur : rdoCharmander.Checked ? PokemonId.Charmander : PokemonId.Squirtle;
            EncounterTutorialCompleteResponse res = null;
            Task.Run(() =>
           {
               res = session.Client.Encounter.EncounterTutorialComplete(firstPoke).Result;

               //await DelayingUtils.DelayAsync(7000, 2000, cancellationToken);
               if (res.Result != EncounterTutorialCompleteResponse.Types.Result.Success)
                   session.EventDispatcher.Send(new NoticeEvent()
                   {
                       Message = $"Caught Tutorial pokemon! it's {firstPoke}!"
                   });
               //return true;

               //state.CatchFirstPokemon(session, session.CancellationTokenSource.Token).Wait();
           }).Wait();

            wizardControl1.NextPage();
        }

        private void wizardPage6_Initialize(object sender, AeroWizard.WizardPageInitEventArgs e)
        {
            string nickname = txtNick.Text;
            ClaimCodenameResponse res = null;


            bool markTutorialComplete = false;
            string errorText = null;
            string warningText = null;
            string infoText = null;
            Task.Run(() =>
            {
                res = session.Client.Misc.ClaimCodename(nickname).Result;

                switch (res.Status)
                {
                    case ClaimCodenameResponse.Types.Status.Unset:
                        errorText = "Unset, somehow";
                        break;
                    case ClaimCodenameResponse.Types.Status.Success:
                        infoText = $"Your name is now: {res.Codename}";
                        markTutorialComplete = true;

                        break;
                    case ClaimCodenameResponse.Types.Status.CodenameNotAvailable:
                        errorText = $"That nickname ({nickname}) isn't available, pick another one!";
                        break;
                    case ClaimCodenameResponse.Types.Status.CodenameNotValid:
                        errorText = $"That nickname ({nickname}) isn't valid, pick another one!";
                        break;
                    case ClaimCodenameResponse.Types.Status.CurrentOwner:
                        warningText = $"You already own that nickname!";
                        markTutorialComplete = true;
                        break;
                    case ClaimCodenameResponse.Types.Status.CodenameChangeNotAllowed:
                        warningText = "You can't change your nickname anymore!";
                        markTutorialComplete = true;
                        break;
                    default:
                        errorText = "Unknown Niantic error while changing nickname.";
                        break;
                }
                if (!string.IsNullOrEmpty(infoText))
                {
                    session.EventDispatcher.Send(new NoticeEvent()
                    {
                        Message = infoText
                    });
                }
                else if (!string.IsNullOrEmpty(warningText))
                {
                    session.EventDispatcher.Send(new WarnEvent()
                    {
                        Message = warningText
                    });
                }
                else if (!string.IsNullOrEmpty(errorText))
                {
                    session.EventDispatcher.Send(new ErrorEvent()
                    {
                        Message = errorText
                    });
                }

                if (markTutorialComplete)
                {

                    var x = session.Client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>()
                  {
                        TutorialState.NameSelection
                  }).Result;
                    session.EventDispatcher.Send(new NoticeEvent()
                    {
                        Message = "First time experience complete, looks like i just spinned an virtual pokestop :P"
                    });
                }

                //await DelayingUtils.DelayAsync(3000, 2000, cancellationToken);
            }).Wait();

            if (markTutorialComplete)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else {
                lblNameError.Text = errorText;
                lblNameError.Visible = true;
                wizardControl1.PreviousPage();
            }

        }

        private void wizardPage1_Initialize(object sender, AeroWizard.WizardPageInitEventArgs e)
        {
            //Task.Run(() =>
            // {
            if (tutState.Contains(TutorialState.AvatarSelection))
            {
                wizardControl1.NextPage(Step2);
            }
            // }
            // ).Wait();
        }

        private void wizardPage2_Initialize(object sender, AeroWizard.WizardPageInitEventArgs e)
        {
            // Task.Run(() =>
            //  {
            if (tutState.Contains(TutorialState.PokemonCapture))
            {

                wizardControl1.NextPage(Step3);

            }
            //  }).Wait();
        }

        private void wizardPage5_Initialize(object sender, AeroWizard.WizardPageInitEventArgs e)
        {
            //Task.Run(() =>
            // {
            if (tutState.Contains(TutorialState.NameSelection))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            // }).Wait();
        }
    }
}
