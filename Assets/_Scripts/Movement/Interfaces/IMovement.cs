using UnityEngine;

public interface IMovement
{
    /// <summary>
    /// Rotate the player
    /// </summary>
    /// <param name="rot">delta rotation</param>
    void Rotate(Quaternion rot);

    /// <summary>
    /// Move the player forward/backwards
    /// </summary>
    /// <param name="translation">delta position</param>
    void Translate(Vector3 translation);
}
