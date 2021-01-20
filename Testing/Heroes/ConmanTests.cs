using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using TheDragonRune.Conman;

namespace TheDragonRuneTest
{
    [TestFixture()]
    public class ConmanTests : CustomBaseTest
    {

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(conman.CharacterCard, 1);
            DealDamage(villain, conman, 2, DamageType.Melee, true);
        }

        [Test()]
        public void TestLoadConman()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(conman);
            Assert.IsInstanceOf(typeof(ConmanCharacterCardController), conman.CharacterCardController);

            Assert.AreEqual(31, conman.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestConmanInnatePower()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //The next time {Conman} would take Damage, redirect it to another  hero target and reduce the damage by 2.
            UsePower(conman.CharacterCard);
            SelectCardsForNextDecision(bunker.CharacterCard);
            QuickHPStorage(baron, conman, legacy, bunker, scholar);
            DealDamage(baron, conman, 5, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -3, 0);

            //only next damage
            QuickHPUpdate();
            DealDamage(baron, conman, 5, DamageType.Melee);
            QuickHPCheck(0, -5, 0, 0, 0);

        }

        [Test()]
        public void TestConmanIncap3()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetupIncap(baron);
            //Select a hero target. The next time that target would be dealt damage, redirect that damage to another target.
            DecisionSelectCards = new Card[] { bunker.CharacterCard, bunker.CharacterCard, baron.CharacterCard };
            UseIncapacitatedAbility(conman, 2);
            //check for no stacking
            UseIncapacitatedAbility(conman, 2);

            QuickHPStorage(baron, legacy, bunker, scholar);
            DealDamage(baron, bunker, 5, DamageType.Fire);
            QuickHPCheck(-5, 0, 0, 0);
            //next only
            QuickHPUpdate();
            DealDamage(baron, bunker, 5, DamageType.Fire);
            QuickHPCheck(0, 0, -5, 0);
        }

        [Test()]
        public void TestEarthIllusion_PlayCard()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(conman);
            Card changeling = PlayCard("WindIllusion");
            AssertInPlayArea(conman, changeling);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card earth = PlayCard("EarthIllusion");
            AssertInDeck(changeling);
            AssertInPlayArea(conman, earth);
            QuickShuffleCheck(1);

            //{Conman} may not Play card or use powers.
            Card handCard = GetRandomCardFromHand(conman);
            AssertCannotPlayCards(conman);
            GoToUsePowerPhase(conman);
            AssertCannotPerformPhaseAction();

            //At the end of {Conman}’s turn, one other player may play a card or use a power.
            DecisionSelectTurnTaker = bunker.TurnTaker;
            Card playCard = PutInHand("AmmoDrop");
            DecisionSelectCard = playCard;
            DecisionSelectFunction = 0;
            GoToEndOfTurn(conman);
            AssertInPlayArea(bunker, playCard);

            //At the start of {Conman}’s turn, you may shuffle this card into the deck.
            DecisionYesNo = true;
            QuickShuffleStorage(conman.TurnTaker.Deck);
            GoToStartOfTurn(conman);
            AssertInDeck(earth);
            QuickShuffleCheck(1);

        }

        [Test()]
        public void TestEarthIllusion_UsePower()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(conman);
            Card changeling = PlayCard("WindIllusion");
            AssertInPlayArea(conman, changeling);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card earth = PlayCard("EarthIllusion");
            AssertInDeck(changeling);
            AssertInPlayArea(conman, earth);
            QuickShuffleCheck(1);

            //{Conman} may not Play card or use powers.
            Card handCard = GetRandomCardFromHand(conman);
            AssertCannotPlayCards(conman);
            GoToUsePowerPhase(conman);
            AssertCannotPerformPhaseAction();

            //At the end of {Conman}’s turn, one other player may play a card or use a power.
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectFunction = 1;
            QuickHandStorage(bunker);
            GoToEndOfTurn(conman);
            QuickHandCheck(1);

            //At the start of {Conman}’s turn, you may shuffle this card into the deck.
            DecisionYesNo = true;
            QuickShuffleStorage(conman.TurnTaker.Deck);
            GoToStartOfTurn(conman);
            AssertInDeck(earth);
            QuickShuffleCheck(1);

        }

        [Test()]
        public void TestEarthIllusion_Optional()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(conman);
            Card changeling = PlayCard("WindIllusion");
            AssertInPlayArea(conman, changeling);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card earth = PlayCard("EarthIllusion");
            AssertInDeck(changeling);
            AssertInPlayArea(conman, earth);
            QuickShuffleCheck(1);

            //{Conman} may not Play card or use powers.
            Card handCard = GetRandomCardFromHand(conman);
            AssertCannotPlayCards(conman);
            GoToUsePowerPhase(conman);
            AssertCannotPerformPhaseAction();

            //At the end of {Conman}’s turn, one other player may play a card or use a power.
            DecisionSelectTurnTaker = bunker.TurnTaker;
            Card playCard = PutInHand("AmmoDrop");
            DecisionSelectCard = playCard;
            DecisionDoNotSelectFunction = true;
            QuickHandStorage(bunker);
            GoToEndOfTurn(conman);
            QuickHandCheckZero();
            AssertInHand(playCard);

            //At the start of {Conman}’s turn, you may shuffle this card into the deck.
            DecisionYesNo = true;
            QuickShuffleStorage(conman.TurnTaker.Deck);
            GoToStartOfTurn(conman);
            AssertInDeck(earth);
            QuickShuffleCheck(1);

        }

    }
}
