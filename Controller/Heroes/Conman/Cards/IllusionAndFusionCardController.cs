using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class IllusionAndFusionCardController : ConmanUtilityCardController
    {

        public IllusionAndFusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsInPlay(new LinqCardCriteria(c => !c.IsCharacter, "", useCardsSuffix: false, singular: "non-character card in play", plural: "non-character cards in play"));
        }

        public override IEnumerator Play()
        {
            //select non-character card
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>() ;
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.ShuffleCardIntoDeck, new LinqCardCriteria(c => !c.IsCharacter && c.IsTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults: storedResults, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(DidSelectCard(storedResults))
            {
                //Shuffle it into their respective deck.
                Card selectedCard = GetSelectedCard(storedResults);
                Location selectedLocation = selectedCard.Location;
                coroutine = GameController.ShuffleCardIntoLocation(HeroTurnTakerController, selectedCard, selectedCard.NativeDeck, false, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if(selectedLocation.IsHeroPlayAreaRecursive)
                {
                    //If that target was in a hero’s Play area, that hero may play a card now.
                    HeroTurnTakerController httc = FindHeroTurnTakerController(selectedLocation.OwnerTurnTaker.ToHero());
                    coroutine = GameController.SelectAndPlayCardFromHand(httc, true, cardSource: GetCardSource());
                }
                else if (IsVillainPlayAreaRecursive(selectedLocation))
                {
                    //If that target in a Villan play area, you may draw a card.

                    coroutine = DrawCard(optional: true);
                }
                else if (IsEnvironmentPlayAreaRecursive(selectedLocation))
                {
                    //If that target was in an Environment play area, you may use a power now.
                    coroutine = GameController.SelectAndUsePower(HeroTurnTakerController, cardSource: GetCardSource());
                } else
                {
                    yield break;
                }
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }

        private bool IsVillainPlayAreaRecursive(Location location)
        {
            if (location.IsPlayArea && location.IsVillain)
            {
                return true;
            }
            if (location.IsNextToCard && location.OwnerCard != null && location.OwnerCard.Location != null)
            {
                return IsVillainPlayAreaRecursive(location.OwnerCard.Location);
            }
            return false; 
        }

        private bool IsEnvironmentPlayAreaRecursive(Location location)
        {
            if (location.IsPlayArea && location.IsEnvironment)
            {
                return true;
            }
            if (location.IsNextToCard && location.OwnerCard != null && location.OwnerCard.Location != null)
            {
                return IsEnvironmentPlayAreaRecursive(location.OwnerCard.Location);
            }
            return false;
        }




    }
}