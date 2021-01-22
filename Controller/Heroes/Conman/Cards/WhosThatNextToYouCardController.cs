using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class WhosThatNextToYouCardController : ConmanUtilityCardController
    {

        public WhosThatNextToYouCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
			//One Villain Target deals 2 melee damage to another target.
			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.DealDamage, new LinqCardCriteria(c => IsVillainTarget(c) && c.IsInPlayAndHasGameText, "villain target"), storedResults, false, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidSelectCard(storedResults))
			{
				Card source = GetSelectedCard(storedResults);
				coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, source), 2, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
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


    }
}