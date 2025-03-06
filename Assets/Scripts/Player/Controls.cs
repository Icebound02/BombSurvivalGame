using UnityEngine;

public enum KeyMaps
{
    Jump,
    Left,
    Right,
    Use
};

public static class Controls
{
    public static KeyCode[][] defaultKeys = new KeyCode[][]
    {
        new KeyCode[]
        {
            KeyCode.W,
            KeyCode.A,
            KeyCode.D,
            KeyCode.E,
        },
        new KeyCode[]
        {
            KeyCode.I,
            KeyCode.J,
            KeyCode.L,
            KeyCode.O,
        },
        new KeyCode[]
        {
            KeyCode.UpArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
            KeyCode.RightShift,
        },
        new KeyCode[]
        {
            KeyCode.Keypad8,
            KeyCode.Keypad4,
            KeyCode.Keypad6,
            KeyCode.Keypad7,
        },
    };

    public static void UpdateControls(Player player)
    {
        for(int i = 0; i < System.Enum.GetValues(typeof(KeyMaps)).Length; ++i)
            player.controls[i] = defaultKeys[player.playerId][i];
    }
}
