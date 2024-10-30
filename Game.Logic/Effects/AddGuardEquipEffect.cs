using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddGuardEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AddGuardEquipEffect(int count, int probability) : base(eEffectType.AddGuardEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddGuardEquipEffect addGuardEquipEffect = living.EffectList.GetOfType(eEffectType.AddGuardEquipEffect) as AddGuardEquipEffect;
			if (addGuardEquipEffect != null)
			{
				addGuardEquipEffect.m_probability = this.m_probability;
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.AddArmor = true;
			player.BeginSelfTurn += new LivingEventHandle(this.player_SelfTurn);
			player.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.Game.SendPlayerPicture(player, BuffType.Defend, true);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.AddArmor = false;
			player.BeginSelfTurn -= new LivingEventHandle(this.player_SelfTurn);
			player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.Game.SendPlayerPicture(player, BuffType.Defend, false);
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
		{
			damageAmount -= this.m_count;
			if ((damageAmount -= this.m_count) <= 0)
			{
				damageAmount = 1;
			}
		}
		private void player_SelfTurn(Living living)
		{
			this.m_probability--;
			if (this.m_probability < 0)
			{
				this.Stop();
			}
		}
	}
}
