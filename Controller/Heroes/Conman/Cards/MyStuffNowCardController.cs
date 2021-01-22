using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class MyStuffNowCardController : ConmanUtilityCardController
    {

        public MyStuffNowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Choose a Villain target, reduce all damage they would deal by X until the start of {Conman}’s turn, where X = the number of Illusions and teleports in {Conman}’s Trash.
            int num = FindCardsWhere(c => TurnTaker.Trash.HasCard(c) && (IsIllusion(c) || IsTeleport(c))).Count();

            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.ModifyDamageAmount, new LinqCardCriteria(c => IsVillainTarget(c) && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "villain targets", useCardsSuffix: false, singular: "villain target", plural: "villain targets"), storedResults: storedResults, false, cardSource: GetCardSource());
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
                Card target = GetSelectedCard(storedResults);
                ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(num);
                effect.SourceCriteria.IsSpecificCard = target;
                effect.UntilTargetLeavesPlay(target);
                effect.UntilStartOfNextTurn(TurnTaker);
                coroutine = AddStatusEffect(effect);
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