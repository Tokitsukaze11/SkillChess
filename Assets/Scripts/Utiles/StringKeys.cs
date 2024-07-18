using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringKeys
{
    public static readonly string MAP_PLACE = "MAP_PLACE";
    public static readonly string DAMAGE = "DAMAGE";
    public static readonly string LINK_EVENT_POPUP_OBJECT = "LINK_EVENT_POPUP";
    public static readonly string HEAL = "HEAL";
    public static readonly string BOX_OBSTACLE = "BOX_OBSTACLE";
    public static readonly string NORMAL_ATTACK_HIT = "NORMAL_ATTACK_HIT";
    public static readonly string ATTACK_SKILL_HIT = "ATTACK_SKILL_HIT";
    public static readonly string MAGIC_MINE = "MAGIC_MINE";
    public static readonly string MAGIC_OTHER = "MAGIC_OTHER";
}
public static class GlobalValues
{
    /// 행
    public static int ROW;
    /// 열
    public static int COL;
    public static Color SELECABLE_COLOUR = Color.yellow;
    public static Color UNSELECT_COLOUR = Color.red;
}