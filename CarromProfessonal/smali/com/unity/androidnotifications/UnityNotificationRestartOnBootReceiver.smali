.class public Lcom/unity/androidnotifications/UnityNotificationRestartOnBootReceiver;
.super Landroid/content/BroadcastReceiver;
.source "UnityNotificationRestartOnBootReceiver.java"


# direct methods
.method public constructor <init>()V
    .locals 0

    .line 12
    invoke-direct {p0}, Landroid/content/BroadcastReceiver;-><init>()V

    return-void
.end method


# virtual methods
.method public onReceive(Landroid/content/Context;Landroid/content/Intent;)V
    .locals 11

    .line 16
    invoke-virtual {p2}, Landroid/content/Intent;->getAction()Ljava/lang/String;

    move-result-object p2

    const-string v0, "android.intent.action.BOOT_COMPLETED"

    invoke-virtual {v0, p2}, Ljava/lang/String;->equals(Ljava/lang/Object;)Z

    move-result p2

    if-eqz p2, :cond_3

    .line 17
    invoke-static {p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->loadNotificationIntents(Landroid/content/Context;)Ljava/util/List;

    move-result-object p2

    .line 19
    invoke-interface {p2}, Ljava/util/List;->iterator()Ljava/util/Iterator;

    move-result-object p2

    :goto_0
    invoke-interface {p2}, Ljava/util/Iterator;->hasNext()Z

    move-result v0

    if-eqz v0, :cond_3

    invoke-interface {p2}, Ljava/util/Iterator;->next()Ljava/lang/Object;

    move-result-object v0

    check-cast v0, Landroid/content/Intent;

    const-string v1, "fireTime"

    const-wide/16 v2, 0x0

    .line 20
    invoke-virtual {v0, v1, v2, v3}, Landroid/content/Intent;->getLongExtra(Ljava/lang/String;J)J

    move-result-wide v4

    .line 21
    invoke-static {}, Ljava/util/Calendar;->getInstance()Ljava/util/Calendar;

    move-result-object v1

    invoke-virtual {v1}, Ljava/util/Calendar;->getTime()Ljava/util/Date;

    move-result-object v1

    .line 22
    new-instance v6, Ljava/util/Date;

    invoke-direct {v6, v4, v5}, Ljava/util/Date;-><init>(J)V

    const/4 v4, -0x1

    const-string v5, "id"

    .line 24
    invoke-virtual {v0, v5, v4}, Landroid/content/Intent;->getIntExtra(Ljava/lang/String;I)I

    move-result v4

    const-string v5, "repeatInterval"

    .line 25
    invoke-virtual {v0, v5, v2, v3}, Landroid/content/Intent;->getLongExtra(Ljava/lang/String;J)J

    move-result-wide v7

    const/4 v5, 0x1

    const/4 v9, 0x0

    cmp-long v10, v7, v2

    if-lez v10, :cond_0

    const/4 v2, 0x1

    goto :goto_1

    :cond_0
    const/4 v2, 0x0

    .line 27
    :goto_1
    invoke-virtual {v6, v1}, Ljava/util/Date;->after(Ljava/util/Date;)Z

    move-result v1

    if-nez v1, :cond_2

    if-eqz v2, :cond_1

    goto :goto_2

    .line 36
    :cond_1
    invoke-static {v4}, Ljava/lang/Integer;->toString(I)Ljava/lang/String;

    move-result-object v0

    invoke-static {p1, v0}, Lcom/unity/androidnotifications/UnityNotificationManager;->deleteExpiredNotificationIntent(Landroid/content/Context;Ljava/lang/String;)V

    goto :goto_0

    .line 28
    :cond_2
    :goto_2
    invoke-static {p1, v5}, Lcom/unity/androidnotifications/UnityNotificationUtilities;->getOpenAppActivity(Landroid/content/Context;Z)Ljava/lang/Class;

    move-result-object v1

    invoke-static {v0, p1, v1}, Lcom/unity/androidnotifications/UnityNotificationManager;->buildOpenAppIntent(Landroid/content/Intent;Landroid/content/Context;Ljava/lang/Class;)Landroid/content/Intent;

    move-result-object v1

    .line 30
    invoke-static {p1, v4, v1, v9}, Lcom/unity/androidnotifications/UnityNotificationManager;->getActivityPendingIntent(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object v1

    .line 31
    invoke-static {p1, v0, v1}, Lcom/unity/androidnotifications/UnityNotificationManager;->buildNotificationIntent(Landroid/content/Context;Landroid/content/Intent;Landroid/app/PendingIntent;)Landroid/content/Intent;

    move-result-object v0

    const/high16 v1, 0x8000000

    .line 33
    invoke-static {p1, v4, v0, v1}, Lcom/unity/androidnotifications/UnityNotificationManager;->getBroadcastPendingIntent(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object v1

    .line 34
    invoke-static {p1, v0, v1}, Lcom/unity/androidnotifications/UnityNotificationManager;->scheduleNotificationIntentAlarm(Landroid/content/Context;Landroid/content/Intent;Landroid/app/PendingIntent;)V

    goto :goto_0

    :cond_3
    return-void
.end method
