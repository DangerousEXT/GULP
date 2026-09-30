using Steamworks;
using Steamworks.Data;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickname;
    [SerializeField] private RawImage avatar;
    [SerializeField] private GameObject hostBadge;

    public async void Setup(Friend friend, bool isHost)
    {
        nickname.text = friend.Name;
        hostBadge.SetActive(isHost);

        var image = await GetAvatar(friend.Id);
        if (image.HasValue)
            avatar.texture = image.Value.Covert();
    }

    private static async Task<Steamworks.Data.Image?> GetAvatar(SteamId id)
    {
        try
        {
            return await SteamFriends.GetLargeAvatarAsync(id);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }
}

public static class SteamImageExtensions
{
    public static Texture2D Covert(this Steamworks.Data.Image image)
    {
        var avatar = new Texture2D((int)image.Width, (int)image.Height, TextureFormat.ARGB32, false);
        avatar.filterMode = FilterMode.Trilinear;

        for (int x = 0; x < image.Width; x++)
            for (int y = 0; y < image.Height; y++)
            {
                var p = image.GetPixel(x, y);
                avatar.SetPixel(x, (int)image.Height - y,
                    new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
            }

        avatar.Apply();
        return avatar;
    }
}