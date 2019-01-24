public interface IAttackable
{
    void ReceiveAttack(float damage);
    void ReceiveStaggerAttack(float damage, UnityEngine.Vector3 staggerDirection, float staggerTime);
    void ReceiveKnockbackAttack(float damage, UnityEngine.Vector3 knockbackDirection, float knockbackVelocity, float knockbackTime);
}