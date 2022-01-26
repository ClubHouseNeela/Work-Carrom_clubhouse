.class public Lcom/unity/androidnotifications/UnityNotificationManagerOreo;
.super Lcom/unity/androidnotifications/UnityNotificationManager;
.source "UnityNotificationManagerOreo.java"


# static fields
.field static final synthetic $assertionsDisabled:Z


# direct methods
.method static constructor <clinit>()V
    .locals 0

    return-void
.end method

.method public constructor <init>(Landroid/content/Context;Landroid/app/Activity;)V
    .locals 0

    .line 13
    invoke-direct {p0, p1, p2}, Lcom/unity/androidnotifications/UnityNotificationManager;-><init>(Landroid/content/Context;Landroid/app/Activity;)V

    return-void
.end method

.method protected static getOreoNotificationChannel(Landroid/content/Context;Ljava/lang/String;)Lcom/unity/androidnotifications/NotificationChannelWrapper;
    .locals 2

    .line 45
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    .line 47
    invoke-static {p0}, Lcom/unity/androidnotifications/UnityNotificationManagerOreo;->getNotificationManager(Landroid/content/Context;)Landroid/app/NotificationManager;

    move-result-object p0

    invoke-virtual {p0}, Landroid/app/NotificationManager;->getNotificationChannels()Ljava/util/List;

    move-result-object p0

    invoke-interface {p0}, Ljava/util/List;->iterator()Ljava/util/Iterator;

    move-result-object p0

    :cond_0
    invoke-interface {p0}, Ljava/util/Iterator;->hasNext()Z

    move-result v0

    if-eqz v0, :cond_1

    invoke-interface {p0}, Ljava/util/Iterator;->next()Ljava/lang/Object;

    move-result-object v0

    check-cast v0, Landroid/app/NotificationChannel;

    .line 48
    invoke-virtual {v0}, Landroid/app/NotificationChannel;->getId()Ljava/lang/String;

    move-result-object v1

    if-ne v1, p1, :cond_0

    .line 49
    invoke-static {v0}, Lcom/unity/androidnotifications/UnityNotificationManagerOreo;->notificationChannelToWrapper(Landroid/app/NotificationChannel;)Lcom/unity/androidnotifications/NotificationChannelWrapper;

    move-result-object p0

    return-object p0

    :cond_1
    const/4 p0, 0x0

    return-object p0
.end method

.method protected static notificationChannelToWrapper(Landroid/app/NotificationChannel;)Lcom/unity/androidnotifications/NotificationChannelWrapper;
    .locals 2

    .line 55
    new-instance v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;

    invoke-direct {v0}, Lcom/unity/androidnotifications/NotificationChannelWrapper;-><init>()V

    .line 57
    invoke-virtual {p0}, Landroid/app/NotificationChannel;->getId()Ljava/lang/String;

    move-result-object v1

    iput-object v1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->id:Ljava/lang/String;

    .line 58
    invoke-virtual {p0}, Landroid/app/NotificationChannel;->getName()Ljava/lang/CharSequence;

    move-result-object v1

    invoke-interface {v1}, Ljava/lang/CharSequence;->toString()Ljava/lang/String;

    move-result-object v1

    iput-object v1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->name:Ljava/lang/String;

    .line 59
    invoke-virtual {p0}, Landroid/app/NotificationChannel;->getImportance()I

    move-result v1

    iput v1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->importance:I

    .line 60
    invoke-virtual {p0}, Landroid/app/NotificationChannel;->getDescription()Ljava/lang/String;

    move-result-object v1

    iput-object v1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->description:Ljava/lang/String;

    .line 61
    invoke-virtual {p0}, Landroid/app/NotificationChannel;->shouldShowLights()Z

    move-result v1

    iput-boolean v1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->enableLights:Z

    .line 62
    invoke-virtual {p0}, Landroid/app/NotificationChannel;->shouldVibrate()Z

    move-result v1

    iput-boolean v1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->enableVibration:Z

    .line 63
    invoke-virtual {p0}, Landroid/app/NotificationChannel;->canBypassDnd()Z

    move-result v1

    iput-boolean v1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->canBypassDnd:Z

    .line 64
    invoke-virtual {p0}, Landroid/app/NotificationChannel;->canShowBadge()Z

    move-result v1

    iput-boolean v1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->canShowBadge:Z

    .line 65
    invoke-virtual {p0}, Landroid/app/NotificationChannel;->getVibrationPattern()[J

    move-result-object v1

    iput-object v1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->vibrationPattern:[J

    .line 66
    invoke-virtual {p0}, Landroid/app/NotificationChannel;->getLockscreenVisibility()I

    move-result p0

    iput p0, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->lockscreenVisibility:I

    return-object v0
.end method


# virtual methods
.method public deleteNotificationChannel(Ljava/lang/String;)V
    .locals 1

    .line 75
    invoke-virtual {p0}, Lcom/unity/androidnotifications/UnityNotificationManagerOreo;->getNotificationManager()Landroid/app/NotificationManager;

    move-result-object v0

    invoke-virtual {v0, p1}, Landroid/app/NotificationManager;->deleteNotificationChannel(Ljava/lang/String;)V

    return-void
.end method

.method public getNotificationChannels()[Lcom/unity/androidnotifications/NotificationChannelWrapper;
    .locals 3

    .line 82
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    .line 84
    invoke-virtual {p0}, Lcom/unity/androidnotifications/UnityNotificationManagerOreo;->getNotificationManager()Landroid/app/NotificationManager;

    move-result-object v1

    invoke-virtual {v1}, Landroid/app/NotificationManager;->getNotificationChannels()Ljava/util/List;

    move-result-object v1

    invoke-interface {v1}, Ljava/util/List;->iterator()Ljava/util/Iterator;

    move-result-object v1

    :goto_0
    invoke-interface {v1}, Ljava/util/Iterator;->hasNext()Z

    move-result v2

    if-eqz v2, :cond_0

    invoke-interface {v1}, Ljava/util/Iterator;->next()Ljava/lang/Object;

    move-result-object v2

    check-cast v2, Landroid/app/NotificationChannel;

    .line 85
    invoke-static {v2}, Lcom/unity/androidnotifications/UnityNotificationManagerOreo;->notificationChannelToWrapper(Landroid/app/NotificationChannel;)Lcom/unity/androidnotifications/NotificationChannelWrapper;

    move-result-object v2

    invoke-interface {v0, v2}, Ljava/util/List;->add(Ljava/lang/Object;)Z

    goto :goto_0

    .line 88
    :cond_0
    invoke-interface {v0}, Ljava/util/List;->size()I

    move-result v1

    new-array v1, v1, [Lcom/unity/androidnotifications/NotificationChannelWrapper;

    invoke-interface {v0, v1}, Ljava/util/List;->toArray([Ljava/lang/Object;)[Ljava/lang/Object;

    move-result-object v0

    check-cast v0, [Lcom/unity/androidnotifications/NotificationChannelWrapper;

    return-object v0
.end method

.method public bridge synthetic getNotificationChannels()[Ljava/lang/Object;
    .locals 1

    .line 11
    invoke-virtual {p0}, Lcom/unity/androidnotifications/UnityNotificationManagerOreo;->getNotificationChannels()[Lcom/unity/androidnotifications/NotificationChannelWrapper;

    move-result-object v0

    return-object v0
.end method

.method public registerNotificationChannel(Ljava/lang/String;Ljava/lang/String;ILjava/lang/String;ZZZZ[JI)V
    .locals 1

    .line 30
    new-instance v0, Landroid/app/NotificationChannel;

    invoke-direct {v0, p1, p2, p3}, Landroid/app/NotificationChannel;-><init>(Ljava/lang/String;Ljava/lang/CharSequence;I)V

    .line 31
    invoke-virtual {v0, p4}, Landroid/app/NotificationChannel;->setDescription(Ljava/lang/String;)V

    .line 32
    invoke-virtual {v0, p5}, Landroid/app/NotificationChannel;->enableLights(Z)V

    .line 33
    invoke-virtual {v0, p6}, Landroid/app/NotificationChannel;->enableVibration(Z)V

    .line 34
    invoke-virtual {v0, p7}, Landroid/app/NotificationChannel;->setBypassDnd(Z)V

    .line 35
    invoke-virtual {v0, p8}, Landroid/app/NotificationChannel;->setShowBadge(Z)V

    .line 36
    invoke-virtual {v0, p9}, Landroid/app/NotificationChannel;->setVibrationPattern([J)V

    .line 37
    invoke-virtual {v0, p10}, Landroid/app/NotificationChannel;->setLockscreenVisibility(I)V

    .line 39
    invoke-virtual {p0}, Lcom/unity/androidnotifications/UnityNotificationManagerOreo;->getNotificationManager()Landroid/app/NotificationManager;

    move-result-object p1

    invoke-virtual {p1, v0}, Landroid/app/NotificationManager;->createNotificationChannel(Landroid/app/NotificationChannel;)V

    return-void
.end method
