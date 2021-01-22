using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class TeleportSpamCardController : ConmanUtilityCardController
    {

        public TeleportSpamCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDamageDealt);
        }

        public static readonly string FirstTimeDamageDealt = "FirstTimeDamageDealt";

        public override void AddTriggers()
        {
            //The first time each turn {Conman} would take damage, redirect it to a Villain Target.
            AddTrigger((DealDamageAction dd) => !HasBeenSetToTrueThisTurn(FirstTimeDamageDealt) && dd.Target == CharacterCard, RedirectResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
        }

		public IEnumerator RedirectResponse(DealDamageAction dd)
		{
			SetCardPropertyToTrueIfRealAction(FirstTimeDamageDealt);
			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.RedirectDamage, new LinqCardCriteria(c => IsVillainTarget(c) && c.IsInPlayAndHasGameText, "villain target"), storedResults, false, cardSource: GetCardSource());
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
				Card target = GetSelectedCard(storedResults);
				IEnumerator redirect = base.GameController.RedirectDamage(dd, target, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(redirect);
				}
				else
				{
					base.GameController.ExhaustCoroutine(redirect);
				}
			}
			yield break;
		}

	}
}