.class final Lcom/unity3d/player/l$2;
.super Ljava/lang/Object;

# interfaces
.implements Ljava/lang/Runnable;


# annotations
.annotation system Ldalvik/annotation/EnclosingMethod;
    value = Lcom/unity3d/player/l;->a(Landroid/content/Context;Ljava/lang/String;IIIZJJLcom/unity3d/player/l$a;)Z
.end annotation

.annotation system Ldalvik/annotation/InnerClass;
    accessFlags = 0x0
    name = null
.end annotation


# instance fields
.field final synthetic a:Lcom/unity3d/player/l;


# direct methods
.method constructor <init>(Lcom/unity3d/player/l;)V
    .locals 0

    iput-object p1, p0, Lcom/unity3d/player/l$2;->a:Lcom/unity3d/player/l;

    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    return-void
.end method


# virtual methods
.method public final run()V
    .locals 1

    iget-object v0, p0, Lcom/unity3d/player/l$2;->a:Lcom/unity3d/player/l;

    invoke-static {v0}, Lcom/unity3d/player/l;->g(Lcom/unity3d/player/l;)Lcom/unity3d/player/UnityPlayer;

    move-result-object v0

    invoke-virtual {v0}, Lcom/unity3d/player/UnityPlayer;->pause()V

    return-void
.end method
