using SharedGame;
using System;
using System.IO;
using Unity.Collections;
using UnityEngine;


using static VWConstants;

public static class VWConstants
{
    public const int MAX_SHIPS = 4;
    public const int MAX_PLAYERS = 64;

    public const int INPUT_F = (1 << 0);
    public const int INPUT_B = (1 << 1);
    public const int INPUT_L = (1 << 2);
    public const int INPUT_R = (1 << 3);
    public const int INPUT_1 = (1 << 4);
    public const int INPUT_2 = (1 << 5);
    public const int INPUT_3 = (1 << 6);
    public const int INPUT_4 = (1 << 7);
    public const int INPUT_5 = (1 << 8);
    public const int INPUT_6 = (1 << 9);



    public const float PI = 3.1415926f;
    public const int STARTING_HEALTH = 100;

}


[Serializable]
public class Ship
{
    public Vector2 position;
    public Vector2 velocity;
    public int health;

    public bool[] dir = new bool[4];
    public bool[] buttons = new bool[6];

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(position.x);
        bw.Write(position.y);
        bw.Write(velocity.x);
        bw.Write(velocity.y);

        bw.Write(health);

    }

    public void Deserialize(BinaryReader br)
    {
        position.x = br.ReadSingle();
        position.y = br.ReadSingle();
        velocity.x = br.ReadSingle();
        velocity.y = br.ReadSingle();
        health = br.ReadInt32();
    }

    public override int GetHashCode()
    {
        int hashCode = 1858597544;
        hashCode = hashCode * -1521134295 + position.GetHashCode();
        hashCode = hashCode * -1521134295 + velocity.GetHashCode();
        hashCode = hashCode * -1521134295 + health.GetHashCode();

        return hashCode;
    }
}

[Serializable]
public struct FgGame : IGame
{
    public int Framenumber { get; private set; }

    public int Checksum => GetHashCode();

    public Ship[] _ships;

    public static Rect _bounds = new Rect(0, 0, 640, 480);

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(Framenumber);
        bw.Write(_ships.Length);
        for (int i = 0; i < _ships.Length; ++i)
        {
            _ships[i].Serialize(bw);
        }
    }

    public void Deserialize(BinaryReader br)
    {
        Framenumber = br.ReadInt32();
        int length = br.ReadInt32();
        if (length != _ships.Length)
        {
            _ships = new Ship[length];
        }
        for (int i = 0; i < _ships.Length; ++i)
        {
            _ships[i].Deserialize(br);
        }
    }

    public NativeArray<byte> ToBytes()
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                Serialize(writer);
            }
            return new NativeArray<byte>(memoryStream.ToArray(), Allocator.Persistent);
        }
    }

    public void FromBytes(NativeArray<byte> bytes)
    {
        using (var memoryStream = new MemoryStream(bytes.ToArray()))
        {
            using (var reader = new BinaryReader(memoryStream))
            {
                Deserialize(reader);
            }
        }
    }

    private static float DegToRad(float deg)
    {
        return PI * deg / 180;
    }

    private static float Distance(Vector2 lhs, Vector2 rhs)
    {
        float x = rhs.x - lhs.x;
        float y = rhs.y - lhs.y;
        return Mathf.Sqrt(x * x + y * y);
    }

    public FgGame(int num_players)
    {
        Framenumber = 0;
        _ships = new Ship[num_players];
        for (int i = 0; i < _ships.Length; i++)
        {
            _ships[i] = new Ship();
            _ships[i].health = STARTING_HEALTH;
        }
    }

    public void GetShipAI(int i, out bool[] directional, out bool[] buttons)
    {
        bool[] temp = new bool[4];
        bool[] tempButtons = new bool[6];

        temp[0] = false;
        temp[1] = false;
        temp[2] = false;
        temp[3] = false;

        buttons = tempButtons;
        directional = temp;
    }

    public void ParseShipInputs(long inputs, int i, out bool[] directional, out bool[] buttons)
    {
        var ship = _ships[i];

        GGPORunner.LogGame($"parsing ship {i} inputs: {inputs}.");

        bool[] temp = new bool[4];
        bool[] but = new bool[6];


        temp[0] = ((inputs & INPUT_L) != 0);
        temp[1] = ((inputs & INPUT_F) != 0);
        temp[2] = ((inputs & INPUT_B) != 0);
        temp[3] = ((inputs & INPUT_R) != 0);

        but[0] = ((inputs & INPUT_1) != 0);
        but[1] = ((inputs & INPUT_2) != 0);
        but[2] = ((inputs & INPUT_3) != 0);
        but[3] = ((inputs & INPUT_4) != 0);
        but[4] = ((inputs & INPUT_5) != 0);
        but[5] = ((inputs & INPUT_6) != 0);

        directional = temp;
        buttons = but;

    }


    public void LogInfo(string filename)
    {
        string fp = "";
        fp += "GameState object.\n";
        fp += string.Format("  bounds: {0},{1} x {2},{3}.\n", _bounds.xMin, _bounds.yMin, _bounds.xMax, _bounds.yMax);
        fp += string.Format("  num_ships: {0}.\n", _ships.Length);
        for (int i = 0; i < _ships.Length; i++)
        {
            var ship = _ships[i];
            fp += string.Format("  ship {0} position:  %.4f, %.4f\n", i, ship.position.x, ship.position.y);
            fp += string.Format("  ship {0} velocity:  %.4f, %.4f\n", i, ship.velocity.x, ship.velocity.y);
            fp += string.Format("  ship {0} health:    %d.\n", i, ship.health);

        }
        File.WriteAllText(filename, fp);
    }

    public void Update(long[] inputs, int disconnect_flags)
    {
        Framenumber++;
        for (int i = 0; i < _ships.Length; i++)
        {
            bool[] dir;
            bool[] buttons;
            if ((disconnect_flags & (1 << i)) != 0)
            {
                GetShipAI(i, out dir, out buttons);
            }
            else
            {
                ParseShipInputs(inputs[i], i, out dir, out buttons);
            }
            for (int j = 0; j < _ships[i].dir.Length; j++)
            {
                _ships[i].dir[j] = dir[j];
                if (i == 0)
                {
                    InputManager.Instance.p1Input.netDirectionals[j] = dir[j];
                }
                else
                {
                    InputManager.Instance.p2Input.netDirectionals[j] = dir[j];
                }
            }

            _ships[i].buttons = buttons;
            if (i == 0) { InputManager.Instance.p1Input.ResolveButtons(buttons); }
            else { InputManager.Instance.p2Input.ResolveButtons(buttons); }
        }
    }

    public long ReadInputs(int id)
    {
        long input = 0;

        // if (id == 0)
        {
            if (InputManager.Instance.p1Input.heldDirectionals[1])
                input |= INPUT_F;
            if (InputManager.Instance.p1Input.heldDirectionals[2])
                input |= INPUT_B;
            if (InputManager.Instance.p1Input.heldDirectionals[0])
                input |= INPUT_L;
            if (InputManager.Instance.p1Input.heldDirectionals[3])
                input |= INPUT_R;
            if (InputManager.Instance.p1Input.heldButtons[0]) { 
                input |= INPUT_1;
            }
            if (InputManager.Instance.p1Input.heldButtons[1])
                input |= INPUT_2;
            if (InputManager.Instance.p1Input.heldButtons[2])
                input |= INPUT_3;
            if (InputManager.Instance.p1Input.heldButtons[3])
                input |= INPUT_4;
            if (InputManager.Instance.p1Input.heldButtons[4])
                input |= INPUT_5;
            if (InputManager.Instance.p1Input.heldButtons[5])
                input |= INPUT_6;
        }
        //else if (id == 1)
        //{
        //    if (InputManager.Instance.p2Input.heldDirectionals[1])
        //        input |= INPUT_F;
        //    if (InputManager.Instance.p2Input.heldDirectionals[2])
        //        input |= INPUT_B;
        //    if (InputManager.Instance.p2Input.heldDirectionals[0])
        //        input |= INPUT_L;
        //    if (InputManager.Instance.p2Input.heldDirectionals[3])
        //        input |= INPUT_R;
        //}

        return input;
    }

    public void FreeBytes(NativeArray<byte> data)
    {
        if (data.IsCreated)
        {
            data.Dispose();
        }
    }

    public override int GetHashCode()
    {
        int hashCode = -1214587014;
        hashCode = hashCode * -1521134295 + Framenumber.GetHashCode();
        foreach (var ship in _ships)
        {
            hashCode = hashCode * -1521134295 + ship.GetHashCode();
        }
        return hashCode;
    }
}
