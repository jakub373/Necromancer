using System.Collections;
using UnityEngine;

public class HomingBullet : Bullet
{
    public AnimationCurve PositionCurve;
    public AnimationCurve NoiseCurve;
    public float yOffset = 1f;
    public Vector2 MinNoise = new Vector2(-3f, -0.25f);
    public Vector2 MaxNoise = new Vector2(3f, 1);

    private Coroutine HomingCoroutine;

    public override void Spawn(Vector3 Forward, float Damage, Transform Target)
    {
        this.damage = Damage;
        this.target = Target;

        if (HomingCoroutine != null)
        {
            StopCoroutine(HomingCoroutine);
        }

        HomingCoroutine = StartCoroutine(FindTarget());
    }

    private IEnumerator FindTarget()
    {
        Vector3 startPosition = transform.position;
        Vector2 Noise = new Vector2(Random.Range(MinNoise.x, MaxNoise.x), Random.Range(MinNoise.y, MaxNoise.y));
        Vector3 BulletDirectionVector = new Vector3(target.position.x, target.position.y + yOffset, target.position.z) - startPosition;
        Vector3 HorizontalNoiseVector = Vector3.Cross(BulletDirectionVector, Vector3.up).normalized;
        float NoisePosition = 0;
        float time = 0;

        while (time < 1)
        {
            NoisePosition = NoiseCurve.Evaluate(time);
            transform.position = Vector3.Lerp(startPosition, target.position + new Vector3(0, yOffset, 0), PositionCurve.Evaluate(time)) + new Vector3(HorizontalNoiseVector.x * NoisePosition * Noise.x, NoisePosition * Noise.y, NoisePosition * HorizontalNoiseVector.z * Noise.x);
            transform.LookAt(target.position + new Vector3(0, yOffset, 0));

            time += Time.deltaTime * MoveSpeed;

            yield return null;
        }
    }
}