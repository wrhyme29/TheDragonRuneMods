using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class WindIllusionCardController : ChangelingCardController
    {

        public WindIllusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //{Conman} may play another card and use an additional power on his turn.

            AddAdditionalPhaseActionTrigger((TurnTaker tt) => ShouldIncreasePhaseActionCount(tt), Phase.PlayCard, 1);
            AddAdditionalPhaseActionTrigger((TurnTaker tt) => ShouldIncreasePhaseActionCount(tt), Phase.UsePower, 1);
        }

		public override IEnumerator Play()
		{
			IEnumerator coroutine = base.Play();
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.PlayCard, 1);
			IEnumerator coroutine2 = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(coroutine2);
			}
		}

		private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
		{
			return tt == base.TurnTaker;
		}

		public override bool AskIfIncreasingCurrentPhaseActionCount()
		{
			if (base.GameController.ActiveTurnPhase.IsPlayCard || base.GameController.ActiveTurnPhase.IsUsePower)
			{
				return ShouldIncreasePhaseActionCount(base.GameController.ActiveTurnTaker);
			}
			return false;
		}


	}
}