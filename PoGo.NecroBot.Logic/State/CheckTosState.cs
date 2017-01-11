#region using directives

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Utils;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Forms;

#endregion

namespace PoGo.NecroBot.Logic.State
{
    public class CheckTosState : IState
    {
        public async Task<IState> Execute(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();


            var tutState = session.Profile.PlayerData.TutorialState;
            if(tutState.Contains(TutorialState.FirstTimeExperienceComplete)) {
                return new InfoState();
            }
            if (!tutState.Contains(TutorialState.LegalScreen))
            {
                await
                    session.Client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>()
                    {
                        TutorialState.LegalScreen
                    });
                session.EventDispatcher.Send(new NoticeEvent()
                {
                    Message = "Just read the Niantic ToS, looks legit, accepting!"
                });
                await DelayingUtils.DelayAsync(5000, 2000, cancellationToken);
            }
            InitialTutorialForm form = new InitialTutorialForm(this,tutState, session);

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

            }
            else
            {
                return new CheckTosState();
            }

            //if (!tutState.Contains(TutorialState.AvatarSelection))
            //{
            //    //string genderString = GlobalSettings.PromptForString(session.Translation, session.Translation.GetTranslation(TranslationString.FirstStartSetupAutoCompleteTutGenderPrompt), new string[] { "Male", "Female" }, "You didn't set a valid gender.", false);

            //    //Gender gen;
            //    //switch (genderString)
            //    //{
            //    //    case "Male":
            //    //    case "male":
            //    //        gen = Gender.Male;
            //    //        break;
            //    //    case "Female":
            //    //    case "female":
            //    //        gen = Gender.Female;
            //    //        break;
            //    //    default:
            //    //        // We should never get here, since the prompt should only allow valid options.
            //    //        gen = Gender.Male;
            //    //        break;
            //    //}
                

            //    //var avatarRes = await session.Client.Player.SetAvatar(new PlayerAvatar()
            //    //{
            //    //    Backpack = 0,
            //    //    Eyes = 0,
            //    //    Gender = gen,
            //    //    Hair = 0,
            //    //    Hat = 0,
            //    //    Pants = 0,
            //    //    Shirt = 0,
            //    //    Shoes = 0,
            //    //    Skin = 0
            //    //});
            //    //if (avatarRes.Status == SetAvatarResponse.Types.Status.AvatarAlreadySet ||
            //    //    avatarRes.Status == SetAvatarResponse.Types.Status.Success)
            //    //{
            //    //    await session.Client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>()
            //    //    {
            //    //        TutorialState.AvatarSelection
            //    //    });
            //    //    session.EventDispatcher.Send(new NoticeEvent()
            //    //    {
            //    //        Message = $"Selected your avatar, now you are {gen}!"
            //    //    });
            //    //}
            //}
            ////if (!tutState.Contains(TutorialState.PokemonCapture))
            ////{
            ////    await CatchFirstPokemon(session, cancellationToken);
            ////}
            //if (!tutState.Contains(TutorialState.NameSelection))
            //{
            //    await SelectNickname(session, cancellationToken);
            //}
            //if (!tutState.Contains(TutorialState.FirstTimeExperienceComplete))
            //{
            //    await
            //        session.Client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>()
            //        {
            //            TutorialState.FirstTimeExperienceComplete
            //        });
            //    session.EventDispatcher.Send(new NoticeEvent()
            //    {
            //        Message = "First time experience complete, looks like i just spinned an virtual pokestop :P"
            //    });
            //    await DelayingUtils.DelayAsync(3000, 2000, cancellationToken);
            //}
            return new InfoState();
        }

        public async Task<bool> CatchFirstPokemon(ISession session, CancellationToken cancellationToken)
        {
            var firstPokeList = new List<PokemonId>
            {
                PokemonId.Bulbasaur,
                PokemonId.Charmander,
                PokemonId.Squirtle
            };
            string pokemonString = GlobalSettings.PromptForString(session.Translation, session.Translation.GetTranslation(TranslationString.FirstStartSetupAutoCompleteTutStarterPrompt), new string[] { "Bulbasaur", "Charmander", "Squirtle" }, "You didn't enter a valid pokemon.", false);
            var firstpokenum = 0;
            switch (pokemonString)
            {
                case "Bulbasaur":
                case "bulbasaur":
                    firstpokenum = 0;
                    break;
                case "Charmander":
                case "charmander":
                    firstpokenum = 1;
                    break;
                case "Squirtle":
                case "squirtle":
                    firstpokenum = 2;
                    break;
                default:
                    // We should never get here.
                    firstpokenum = 0;
                    break;
            }
            
            var firstPoke = firstPokeList[firstpokenum];

            var res = await session.Client.Encounter.EncounterTutorialComplete(firstPoke);
            await DelayingUtils.DelayAsync(7000, 2000, cancellationToken);
            if (res.Result != EncounterTutorialCompleteResponse.Types.Result.Success) return false;
            session.EventDispatcher.Send(new NoticeEvent()
            {
                Message = $"Caught Tutorial pokemon! it's {firstPoke}!"
            });
            return true;
        }

        public async Task<bool> SelectNickname(ISession session, CancellationToken cancellationToken)
        {
            while (true)
            {
                string nickname = GlobalSettings.PromptForString(session.Translation, session.Translation.GetTranslation(TranslationString.FirstStartSetupAutoCompleteTutNicknamePrompt), null, "You entered an invalid nickname.");

                if (nickname.Length > 15 || nickname.Length == 0)
                {
                    session.EventDispatcher.Send(new ErrorEvent()
                    {
                        Message = "Your desired nickname is too long (max length 15 characters)!"
                    });
                    continue;
                }

                var res = await session.Client.Misc.ClaimCodename(nickname);

                bool markTutorialComplete = false;
                string errorText = null;
                string warningText = null;
                string infoText = null;
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
                    await session.Client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>()
                    {
                        TutorialState.NameSelection
                    });

                    await DelayingUtils.DelayAsync(3000, 2000, cancellationToken);
                    return res.Status == ClaimCodenameResponse.Types.Status.Success;
                }
            }
        }
    }
}