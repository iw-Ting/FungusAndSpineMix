using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fungus {
    public static class FungusResources //��� fungus prefab�귽���}��
    {

        public static IEnumerator GetCharacter(string charaName,Action<Character>finish) {

            ResourceRequest request = Resources.LoadAsync<GameObject>(FungusResourcesPath.Chara2DPortraits+charaName);
            yield return request;

            finish((request.asset as GameObject).GetComponent<Character>());
      
        }

        public static IEnumerator GetCharaSpine(string charaName, Action<CharaSpine> finish)
        {

            ResourceRequest request = Resources.LoadAsync<GameObject>(FungusResourcesPath.SpineChara + charaName);
            yield return request;

            finish( (request.asset as GameObject).GetComponent<CharaSpine>() );

        }

        
    }


}