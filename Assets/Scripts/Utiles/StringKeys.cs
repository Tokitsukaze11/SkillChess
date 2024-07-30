using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringKeys
{
    public static readonly string MAP_PLACE = "MAP_PLACE";
    public static readonly string DAMAGE = "DAMAGE";
    public static readonly string LINK_EVENT_POPUP_OBJECT = "LINK_EVENT_POPUP";
    public static readonly string HEAL = "HEAL";
    public static readonly string NORMAL_ATTACK_HIT = "NORMAL_ATTACK_HIT";
    public static readonly string ATTACK_SKILL_HIT = "ATTACK_SKILL_HIT";
    public static readonly string MAGIC_MINE = "MAGIC_MINE";
    public static readonly string MAGIC_OTHER = "MAGIC_OTHER";
    public static readonly string HEAL_PARTICLE = "HEAL_PARTICLE";
    public static readonly string TOWER_A_OBSTACLE = "TOWER_A_OBSTACLE";
    public static readonly string TOWER_B_OBSTACLE = "TOWER_B_OBSTACLE";
    public static readonly string TOWER_C_OBSTACLE = "TOWER_C_OBSTACLE";
    public static readonly string WALL_B_OBSTACLE = "WALL_B_OBSTACLE";
}
public static class GlobalValues
{
    /// 행
    public static int ROW;
    /// 열
    public static int COL;
    public static Color ATTACKABLE_COLOUR = new Color(1f, 0, 0, 50/255f); // Attackable
    public static Color MOVEABLE_COLOUR = new Color(0f, 229/255f, 255/255f, 0.6f);
    public static Color UNSELECT_COLOUR = Color.clear;
}