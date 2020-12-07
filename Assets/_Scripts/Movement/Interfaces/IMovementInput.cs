public interface IMovementInput
{
    /// <summary>
    /// Input rather to rotate left or right
    /// </summary>
    /// <returns> 
    ///     <br>0..no rotation input</br>
    ///     <br>1..positive rotation</br>
    ///     <br>-1..negative rotation</br>
    /// </returns>
    float RotationInput();

    /// <summary>
    /// Input rather to move left or right
    /// </summary>
    /// <returns> 
    ///     <br>0..no movement input</br>
    ///     <br>1..forward movement</br>
    ///     <br>-1..backwards movement</br>
    /// </returns>
    float TranslationInput();
}
