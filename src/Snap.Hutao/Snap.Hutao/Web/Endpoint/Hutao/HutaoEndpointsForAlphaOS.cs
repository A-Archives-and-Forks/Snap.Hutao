// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

[Injection(InjectAs.Singleton, typeof(IHutaoEndpoints), Key = HutaoEndpointsKind.AlphaOS)]
internal sealed class HutaoEndpointsForAlphaOS : IHutaoEndpoints
{
    string IHomaRootAccess.Root { get => "https://homa.snapgenshin.com"; }

    string IInfrastructureRootAccess.Root { get => "https://alpha.snapgenshin.cn/global"; }

    public string PatchSnapHutao()
    {
        return $"{((IInfrastructureRootAccess)this).Root}/patch/alpha";
    }
}