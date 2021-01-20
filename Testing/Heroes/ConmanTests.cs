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

    }
}
