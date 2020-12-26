
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{


	public class MrNobleCharacterCardController : VillainCharacterCardController
	{
		private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";

		private const string FlippedToBack = "FlippedToBack";

		private const string FlippedToFront = "FlippedToFront";

		private ITrigger _reduceDamageTrigger;

		public MrNobleCharacterCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowIfElseSpecialString(() => WasFlippedToBackThisTurn(), () => base.Card.AlternateTitleOrTitle + " has already flipped to the back this turn.", () => base.Card.AlternateTitleOrTitle + " has not flipped to the back this turn.").Condition = () => base.Game.HasGameStarted && !base.Card.IsFlipped;
			base.SpecialStringMaker.ShowIfElseSpecialString(() => WasFlippedToFrontThisTurn(), () => base.Card.AlternateTitleOrTitle + " has already flipped to the front this turn.", () => base.Card.AlternateTitleOrTitle + " has not flipped to the front this turn.").Condition = () => base.Card.IsFlipped;
			base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsHighTierEnforcer(c), "high tier enforcer")).Condition = () => !base.Card.IsFlipped;
			base.SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisTurn("FirstTimeDamageDealt"), () => base.Card.AlternateTitleOrTitle + "'s reduce damage effect has been used this turn.", () => base.Card.AlternateTitleOrTitle + "'s reduce damage effect has not been used this turn.").Condition = () => base.Game.HasGameStarted && !base.Card.IsFlipped;
			base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsEnforcer(c), "enforcer"));
			base.SpecialStringMaker.ShowHighestHP(1, null, new LinqCardCriteria((Card c) => IsEnforcer(c) && c.IsInPlayAndHasGameText, "enforcer")).Condition = () => !base.Card.IsFlipped;
			base.SpecialStringMaker.ShowHeroCharacterCardWithHighestHP(1, 2).Condition = () => !base.Card.IsFlipped;
			base.SpecialStringMaker.ShowNumberOfCards(new LinqCardCriteria((Card c) => c.IsIncapacitated && c.IsHero, "incapicated hero")).Condition = () => base.Card.IsFlipped;
			base.SpecialStringMaker.ShowHeroCharacterCardWithLowestHP(2).Condition = () => base.Card.IsFlipped;
		}

		public override void AddSideTriggers()
		{
			if (!base.Card.IsFlipped)
			{
				AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, FlipFromFrontResponse, TriggerType.FlipCard, (PhaseChangeAction pca) => GetNumberOfHighTierEnforcersInPlay() == 0 && !WasFlippedToBackThisTurn()));
				AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && IsEnforcer(dd.DamageSource.Card), 2));
				_reduceDamageTrigger = AddTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.Amount > 0 && !HasBeenSetToTrueThisTurn("FirstTimeDamageDealt"), ReduceDamageFrontResponse, TriggerType.ReduceDamage, TriggerTiming.Before);
				AddSideTrigger(_reduceDamageTrigger);
				AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, EndOfTurnFrontResponse, TriggerType.DealDamage));
			}
			else
			{
				AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, FlipFromBackResponse, new TriggerType[2]
				{
				TriggerType.DealDamage,
				TriggerType.FlipCard
				}, (PhaseChangeAction pca) => GetNumberOfEnforcersInPlay() >= base.H - 1 && !WasFlippedToFrontThisTurn()));
				AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card == base.Card, (DealDamageAction dd) => GetNumberOfIncapicatedHeroes()));
				AddSideTrigger(AddReduceDamageTrigger((DealDamageAction dd) => dd.Target == base.Card, (DealDamageAction dd) => GetNumberOfIncapicatedHeroes()));
				AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, EndOfTurnBackResponse, new TriggerType[2]
				{
				TriggerType.DealDamage,
				TriggerType.PlayCard
				}));
			}
			AddDefeatedIfDestroyedTriggers();
			AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("FirstTimeDamageDealt"), TriggerType.Hidden);
			AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("FlippedToBack"), TriggerType.Hidden);
			AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("FlippedToFront"), TriggerType.Hidden);
		}

		private IEnumerator ReduceDamageFrontResponse(DealDamageAction dd)
		{
			SetCardPropertyToTrueIfRealAction("FirstTimeDamageDealt");
			IEnumerator coroutine = base.GameController.ReduceDamage(dd, GetNumberOfEnforcersInPlay(), _reduceDamageTrigger, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private IEnumerator EndOfTurnBackResponse(PhaseChangeAction pca)
		{
			List<DealDamageAction> storedResults = new List<DealDamageAction>();
			_ = base.GameController.Game.H;
			IEnumerator coroutine2 = DealDamageToLowestHP(base.Card, 2, (Card c) => c.IsHeroCharacterCard, (Card c) => base.H - 2, DamageType.Melee, isIrreducible: false, optional: false, storedResults);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			DealDamageAction dealDamageAction = storedResults.FirstOrDefault();
			if (dealDamageAction != null && dealDamageAction.OriginalTarget != null)
			{
				coroutine2 = DealDamage(base.Card, dealDamageAction.OriginalTarget, base.H - 2, DamageType.Projectile, isIrreducible: false, optional: false, isCounterDamage: false, null, null, null, ignoreBattleZone: false, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			coroutine2 = PlayTheTopCardOfTheVillainDeckWithMessageResponse(pca);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
		}

		public override IEnumerator AfterFlipCardImmediateResponse()
		{
			IEnumerator coroutine2 = base.AfterFlipCardImmediateResponse();
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			if (base.Card.IsFlipped)
			{
				coroutine2 = DealDamage(base.Card, (Card c) => c.IsHero && c.IsTarget, base.H - 2, DamageType.Projectile);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
		}

		private IEnumerator FlipFromBackResponse(PhaseChangeAction pca)
		{
			SetCardPropertyToTrueIfRealAction("FlippedToFront");
			IEnumerator coroutine2 = DealDamage(base.Card, (Card c) => c.IsHero && c.IsTarget, 1, DamageType.Melee);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			coroutine2 = FlipThisCharacterCardResponse(pca);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
		}

		private IEnumerator FlipFromFrontResponse(PhaseChangeAction pca)
		{
			SetCardPropertyToTrueIfRealAction("FlippedToBack");
			IEnumerator coroutine = FlipThisCharacterCardResponse(pca);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private IEnumerator EndOfTurnFrontResponse(PhaseChangeAction arg)
		{
			IEnumerator coroutine = DealDamageToHighestHP(damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => IsEnforcer(c) && c.IsTarget, "the enforcer with the highest HP")), source: null, ranking: 1, targetCriteria: (Card c) => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, amount: (Card c) => base.H - 1, type: DamageType.Projectile, isIrreducible: false, optional: false, storedResults: null, numberOfTargets: () => 2);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private bool IsEnforcer(Card card)
		{
			return card != null && (base.GameController.DoesCardContainKeyword(card, "high tier enforcer") || base.GameController.DoesCardContainKeyword(card, "mid tier enforcer") || base.GameController.DoesCardContainKeyword(card, "lower tier enforcer"));
		}

		private bool IsHighTierEnforcer(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "high tier enforcer");
		}

		private int GetNumberOfEnforcersInPlay()
		{
			return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsEnforcer(c)).Count();
		}

		private int GetNumberOfHighTierEnforcersInPlay()
		{
			return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsHighTierEnforcer(c)).Count();
		}

		private int GetNumberOfIncapicatedHeroes()
		{
			return FindCardsWhere((Card c) => c.IsIncapacitated && c.IsHeroCharacterCard).Count();
		}

		private bool WasFlippedToBackThisTurn()
		{
			return base.Game.Journal.CardPropertiesEntriesThisTurn(base.Card).Any((CardPropertiesJournalEntry j) => j.Key == "FlippedToBack");
		}

		private bool WasFlippedToFrontThisTurn()
		{
			return base.Game.Journal.CardPropertiesEntriesThisTurn(base.Card).Any((CardPropertiesJournalEntry j) => j.Key == "FlippedToFront");
		}
	}

}