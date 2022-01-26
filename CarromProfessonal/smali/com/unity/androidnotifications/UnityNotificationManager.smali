.class public Lcom/unity/androidnotifications/UnityNotificationManager;
.super Landroid/content/BroadcastReceiver;
.source "UnityNotificationManager.java"


# static fields
.field protected static final NOTIFICATION_CHANNELS_SHARED_PREFS:Ljava/lang/String; = "UNITY_NOTIFICATIONS"

.field protected static final NOTIFICATION_CHANNELS_SHARED_PREFS_KEY:Ljava/lang/String; = "ChannelIDs"

.field protected static final NOTIFICATION_IDS_SHARED_PREFS:Ljava/lang/String; = "UNITY_STORED_NOTIFICATION_IDS"

.field protected static final NOTIFICATION_IDS_SHARED_PREFS_KEY:Ljava/lang/String; = "UNITY_NOTIFICATION_IDS"

.field protected static mNotificationCallback:Lcom/unity/androidnotifications/NotificationCallback;

.field protected static mUnityNotificationManager:Lcom/unity/androidnotifications/UnityNotificationManager;


# instance fields
.field protected mActivity:Landroid/app/Activity;

.field public mContext:Landroid/content/Context;

.field protected mOpenActivity:Ljava/lang/Class;

.field protected mRescheduleOnRestart:Z


# direct methods
.method public constructor <init>()V
    .locals 1

    .line 47
    invoke-direct {p0}, Landroid/content/BroadcastReceiver;-><init>()V

    const/4 v0, 0x0

    .line 35
    iput-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    .line 36
    iput-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mActivity:Landroid/app/Activity;

    .line 37
    iput-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mOpenActivity:Ljava/lang/Class;

    const/4 v0, 0x0

    .line 38
    iput-boolean v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mRescheduleOnRestart:Z

    return-void
.end method

.method public constructor <init>(Landroid/content/Context;Landroid/app/Activity;)V
    .locals 6

    const-string v0, "UnityNotifications"

    .line 52
    invoke-direct {p0}, Landroid/content/BroadcastReceiver;-><init>()V

    const/4 v1, 0x0

    .line 35
    iput-object v1, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    .line 36
    iput-object v1, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mActivity:Landroid/app/Activity;

    .line 37
    iput-object v1, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mOpenActivity:Ljava/lang/Class;

    const/4 v1, 0x0

    .line 38
    iput-boolean v1, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mRescheduleOnRestart:Z

    .line 53
    iput-object p1, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    .line 54
    iput-object p2, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mActivity:Landroid/app/Activity;

    .line 57
    :try_start_0
    invoke-virtual {p2}, Landroid/app/Activity;->getPackageManager()Landroid/content/pm/PackageManager;

    move-result-object v2

    invoke-virtual {p2}, Landroid/app/Activity;->getPackageName()Ljava/lang/String;

    move-result-object v3

    const/16 v4, 0x80

    invoke-virtual {v2, v3, v4}, Landroid/content/pm/PackageManager;->getApplicationInfo(Ljava/lang/String;I)Landroid/content/pm/ApplicationInfo;

    move-result-object v2

    .line 58
    iget-object v2, v2, Landroid/content/pm/ApplicationInfo;->metaData:Landroid/os/Bundle;

    const-string v3, "reschedule_notifications_on_restart"

    .line 60
    invoke-virtual {v2, v3}, Landroid/os/Bundle;->getBoolean(Ljava/lang/String;)Z

    move-result v2

    invoke-static {v2}, Ljava/lang/Boolean;->valueOf(Z)Ljava/lang/Boolean;

    move-result-object v2

    .line 62
    invoke-virtual {v2}, Ljava/lang/Boolean;->booleanValue()Z

    move-result v3

    if-eqz v3, :cond_0

    .line 63
    new-instance v3, Landroid/content/ComponentName;

    const-class v4, Lcom/unity/androidnotifications/UnityNotificationRestartOnBootReceiver;

    invoke-direct {v3, p1, v4}, Landroid/content/ComponentName;-><init>(Landroid/content/Context;Ljava/lang/Class;)V

    .line 64
    invoke-virtual {p1}, Landroid/content/Context;->getPackageManager()Landroid/content/pm/PackageManager;

    move-result-object v4

    const/4 v5, 0x1

    .line 66
    invoke-virtual {v4, v3, v5, v5}, Landroid/content/pm/PackageManager;->setComponentEnabledSetting(Landroid/content/ComponentName;II)V

    .line 71
    :cond_0
    invoke-virtual {v2}, Ljava/lang/Boolean;->booleanValue()Z

    move-result v2

    iput-boolean v2, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mRescheduleOnRestart:Z

    .line 73
    invoke-static {p1, v1}, Lcom/unity/androidnotifications/UnityNotificationUtilities;->getOpenAppActivity(Landroid/content/Context;Z)Ljava/lang/Class;

    move-result-object p1

    iput-object p1, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mOpenActivity:Ljava/lang/Class;

    if-nez p1, :cond_1

    .line 75
    invoke-virtual {p2}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object p1

    iput-object p1, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mOpenActivity:Ljava/lang/Class;
    :try_end_0
    .catch Landroid/content/pm/PackageManager$NameNotFoundException; {:try_start_0 .. :try_end_0} :catch_1
    .catch Ljava/lang/NullPointerException; {:try_start_0 .. :try_end_0} :catch_0

    goto :goto_0

    :catch_0
    move-exception p1

    .line 79
    new-instance p2, Ljava/lang/StringBuilder;

    invoke-direct {p2}, Ljava/lang/StringBuilder;-><init>()V

    const-string v1, "Failed to load meta-data, NullPointer: "

    invoke-virtual {p2, v1}, Ljava/lang/StringBuilder;->append(Ljava/lang/String;)Ljava/lang/StringBuilder;

    invoke-virtual {p1}, Ljava/lang/NullPointerException;->getMessage()Ljava/lang/String;

    move-result-object p1

    invoke-virtual {p2, p1}, Ljava/lang/StringBuilder;->append(Ljava/lang/String;)Ljava/lang/StringBuilder;

    invoke-virtual {p2}, Ljava/lang/StringBuilder;->toString()Ljava/lang/String;

    move-result-object p1

    invoke-static {v0, p1}, Landroid/util/Log;->e(Ljava/lang/String;Ljava/lang/String;)I

    goto :goto_0

    :catch_1
    move-exception p1

    .line 77
    new-instance p2, Ljava/lang/StringBuilder;

    invoke-direct {p2}, Ljava/lang/StringBuilder;-><init>()V

    const-string v1, "Failed to load meta-data, NameNotFound: "

    invoke-virtual {p2, v1}, Ljava/lang/StringBuilder;->append(Ljava/lang/String;)Ljava/lang/StringBuilder;

    invoke-virtual {p1}, Landroid/content/pm/PackageManager$NameNotFoundException;->getMessage()Ljava/lang/String;

    move-result-object p1

    invoke-virtual {p2, p1}, Ljava/lang/StringBuilder;->append(Ljava/lang/String;)Ljava/lang/StringBuilder;

    invoke-virtual {p2}, Ljava/lang/StringBuilder;->toString()Ljava/lang/String;

    move-result-object p1

    invoke-static {v0, p1}, Landroid/util/Log;->e(Ljava/lang/String;Ljava/lang/String;)I

    :cond_1
    :goto_0
    return-void
.end method

.method protected static buildNotification(Landroid/content/Context;Landroid/content/Intent;)Landroid/app/Notification$Builder;
    .locals 9

    const-string v0, "channelID"

    .line 522
    invoke-virtual {p1, v0}, Landroid/content/Intent;->getStringExtra(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v0

    .line 525
    sget v1, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 v2, 0x1a

    if-ge v1, v2, :cond_0

    .line 526
    new-instance v1, Landroid/app/Notification$Builder;

    invoke-direct {v1, p0}, Landroid/app/Notification$Builder;-><init>(Landroid/content/Context;)V

    goto :goto_0

    .line 528
    :cond_0
    new-instance v1, Landroid/app/Notification$Builder;

    invoke-direct {v1, p0, v0}, Landroid/app/Notification$Builder;-><init>(Landroid/content/Context;Ljava/lang/String;)V

    :goto_0
    const-string v3, "largeIconStr"

    .line 531
    invoke-virtual {p1, v3}, Landroid/content/Intent;->getStringExtra(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v3

    .line 532
    invoke-static {p0, v3}, Lcom/unity/androidnotifications/UnityNotificationUtilities;->findResourceIdInContextByName(Landroid/content/Context;Ljava/lang/String;)I

    move-result v3

    if-eqz v3, :cond_1

    .line 534
    invoke-virtual {p0}, Landroid/content/Context;->getResources()Landroid/content/res/Resources;

    move-result-object v4

    invoke-static {v4, v3}, Landroid/graphics/BitmapFactory;->decodeResource(Landroid/content/res/Resources;I)Landroid/graphics/Bitmap;

    move-result-object v3

    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setLargeIcon(Landroid/graphics/Bitmap;)Landroid/app/Notification$Builder;

    :cond_1
    const-string v3, "smallIconStr"

    .line 537
    invoke-virtual {p1, v3}, Landroid/content/Intent;->getStringExtra(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v3

    .line 538
    invoke-static {p0, v3}, Lcom/unity/androidnotifications/UnityNotificationUtilities;->findResourceIdInContextByName(Landroid/content/Context;Ljava/lang/String;)I

    move-result v3

    if-nez v3, :cond_2

    .line 540
    invoke-virtual {p0}, Landroid/content/Context;->getApplicationInfo()Landroid/content/pm/ApplicationInfo;

    move-result-object v3

    iget v3, v3, Landroid/content/pm/ApplicationInfo;->icon:I

    .line 542
    :cond_2
    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setSmallIcon(I)Landroid/app/Notification$Builder;

    const-string v3, "textTitle"

    .line 544
    invoke-virtual {p1, v3}, Landroid/content/Intent;->getStringExtra(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v3

    const-string v4, "textContent"

    .line 545
    invoke-virtual {p1, v4}, Landroid/content/Intent;->getStringExtra(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v4

    const-string v5, "tapIntent"

    .line 546
    invoke-virtual {p1, v5}, Landroid/content/Intent;->getParcelableExtra(Ljava/lang/String;)Landroid/os/Parcelable;

    move-result-object v5

    check-cast v5, Landroid/app/PendingIntent;

    const-string v6, "autoCancel"

    const/4 v7, 0x1

    .line 547
    invoke-virtual {p1, v6, v7}, Landroid/content/Intent;->getBooleanExtra(Ljava/lang/String;Z)Z

    move-result v6

    .line 549
    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setContentTitle(Ljava/lang/CharSequence;)Landroid/app/Notification$Builder;

    move-result-object v3

    .line 550
    invoke-virtual {v3, v4}, Landroid/app/Notification$Builder;->setContentText(Ljava/lang/CharSequence;)Landroid/app/Notification$Builder;

    move-result-object v3

    .line 551
    invoke-virtual {v3, v5}, Landroid/app/Notification$Builder;->setContentIntent(Landroid/app/PendingIntent;)Landroid/app/Notification$Builder;

    move-result-object v3

    .line 552
    invoke-virtual {v3, v6}, Landroid/app/Notification$Builder;->setAutoCancel(Z)Landroid/app/Notification$Builder;

    const-string v3, "number"

    const/4 v5, 0x0

    .line 554
    invoke-virtual {p1, v3, v5}, Landroid/content/Intent;->getIntExtra(Ljava/lang/String;I)I

    move-result v3

    if-ltz v3, :cond_3

    .line 556
    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setNumber(I)Landroid/app/Notification$Builder;

    :cond_3
    const-string v3, "style"

    .line 558
    invoke-virtual {p1, v3, v5}, Landroid/content/Intent;->getIntExtra(Ljava/lang/String;I)I

    move-result v3

    const/4 v6, 0x2

    if-ne v3, v6, :cond_4

    .line 560
    new-instance v3, Landroid/app/Notification$BigTextStyle;

    invoke-direct {v3}, Landroid/app/Notification$BigTextStyle;-><init>()V

    invoke-virtual {v3, v4}, Landroid/app/Notification$BigTextStyle;->bigText(Ljava/lang/CharSequence;)Landroid/app/Notification$BigTextStyle;

    move-result-object v3

    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setStyle(Landroid/app/Notification$Style;)Landroid/app/Notification$Builder;

    :cond_4
    const-wide/16 v3, -0x1

    const-string v8, "timestamp"

    .line 562
    invoke-virtual {p1, v8, v3, v4}, Landroid/content/Intent;->getLongExtra(Ljava/lang/String;J)J

    move-result-wide v3

    .line 563
    invoke-virtual {v1, v3, v4}, Landroid/app/Notification$Builder;->setWhen(J)Landroid/app/Notification$Builder;

    .line 565
    sget v3, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 v4, 0x14

    if-lt v3, v4, :cond_7

    const-string v3, "group"

    .line 566
    invoke-virtual {p1, v3}, Landroid/content/Intent;->getStringExtra(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v3

    if-eqz v3, :cond_5

    .line 567
    invoke-virtual {v3}, Ljava/lang/String;->length()I

    move-result v4

    if-lez v4, :cond_5

    .line 568
    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setGroup(Ljava/lang/String;)Landroid/app/Notification$Builder;

    :cond_5
    const-string v3, "groupSummary"

    .line 571
    invoke-virtual {p1, v3, v5}, Landroid/content/Intent;->getBooleanExtra(Ljava/lang/String;Z)Z

    move-result v3

    if-eqz v3, :cond_6

    .line 573
    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setGroupSummary(Z)Landroid/app/Notification$Builder;

    :cond_6
    const-string v3, "sortKey"

    .line 575
    invoke-virtual {p1, v3}, Landroid/content/Intent;->getStringExtra(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v3

    if-eqz v3, :cond_7

    .line 576
    invoke-virtual {v3}, Ljava/lang/String;->length()I

    move-result v4

    if-lez v4, :cond_7

    .line 577
    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setSortKey(Ljava/lang/String;)Landroid/app/Notification$Builder;

    .line 581
    :cond_7
    sget v3, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 v4, 0x11

    if-lt v3, v4, :cond_8

    const-string v3, "showTimestamp"

    .line 582
    invoke-virtual {p1, v3, v5}, Landroid/content/Intent;->getBooleanExtra(Ljava/lang/String;Z)Z

    move-result v3

    .line 583
    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setShowWhen(Z)Landroid/app/Notification$Builder;

    .line 586
    :cond_8
    sget v3, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 v4, 0x15

    if-lt v3, v4, :cond_9

    const-string v3, "color"

    .line 587
    invoke-virtual {p1, v3, v5}, Landroid/content/Intent;->getIntExtra(Ljava/lang/String;I)I

    move-result v3

    if-eqz v3, :cond_9

    .line 589
    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setColor(I)Landroid/app/Notification$Builder;

    .line 590
    sget v3, Landroid/os/Build$VERSION;->SDK_INT:I

    if-lt v3, v2, :cond_9

    .line 591
    invoke-virtual {v1, v7}, Landroid/app/Notification$Builder;->setColorized(Z)Landroid/app/Notification$Builder;

    .line 596
    :cond_9
    sget v3, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 v7, 0x16

    if-lt v3, v7, :cond_a

    const-string v3, "usesChronometer"

    .line 597
    invoke-virtual {p1, v3, v5}, Landroid/content/Intent;->getBooleanExtra(Ljava/lang/String;Z)Z

    move-result v3

    .line 598
    invoke-virtual {v1, v3}, Landroid/app/Notification$Builder;->setUsesChronometer(Z)Landroid/app/Notification$Builder;

    .line 601
    :cond_a
    sget v3, Landroid/os/Build$VERSION;->SDK_INT:I

    if-ge v3, v2, :cond_11

    .line 603
    invoke-static {p0, v0}, Lcom/unity/androidnotifications/UnityNotificationManager;->getNotificationChannel(Landroid/content/Context;Ljava/lang/String;)Lcom/unity/androidnotifications/NotificationChannelWrapper;

    move-result-object p0

    .line 605
    iget-object p1, p0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->vibrationPattern:[J

    const/4 v0, -0x1

    if-eqz p1, :cond_b

    iget-object p1, p0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->vibrationPattern:[J

    array-length p1, p1

    if-lez p1, :cond_b

    const/4 p1, 0x5

    .line 606
    invoke-virtual {v1, p1}, Landroid/app/Notification$Builder;->setDefaults(I)Landroid/app/Notification$Builder;

    .line 607
    iget-object p1, p0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->vibrationPattern:[J

    invoke-virtual {v1, p1}, Landroid/app/Notification$Builder;->setVibrate([J)Landroid/app/Notification$Builder;

    goto :goto_1

    .line 609
    :cond_b
    invoke-virtual {v1, v0}, Landroid/app/Notification$Builder;->setDefaults(I)Landroid/app/Notification$Builder;

    .line 612
    :goto_1
    sget p1, Landroid/os/Build$VERSION;->SDK_INT:I

    if-lt p1, v4, :cond_c

    .line 613
    iget p1, p0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->lockscreenVisibility:I

    invoke-virtual {v1, p1}, Landroid/app/Notification$Builder;->setVisibility(I)Landroid/app/Notification$Builder;

    .line 618
    :cond_c
    iget p0, p0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->importance:I

    if-eqz p0, :cond_f

    if-eq p0, v6, :cond_e

    const/4 p1, 0x3

    if-eq p0, p1, :cond_10

    const/4 p1, 0x4

    if-eq p0, p1, :cond_d

    goto :goto_2

    :cond_d
    const/4 v5, 0x2

    goto :goto_2

    :cond_e
    const/4 v5, -0x1

    goto :goto_2

    :cond_f
    const/4 v5, -0x2

    .line 634
    :cond_10
    :goto_2
    invoke-virtual {v1, v5}, Landroid/app/Notification$Builder;->setPriority(I)Landroid/app/Notification$Builder;

    goto :goto_3

    :cond_11
    const-string p0, "groupAlertBehaviour"

    .line 637
    invoke-virtual {p1, p0, v5}, Landroid/content/Intent;->getIntExtra(Ljava/lang/String;I)I

    move-result p0

    .line 638
    invoke-virtual {v1, p0}, Landroid/app/Notification$Builder;->setGroupAlertBehavior(I)Landroid/app/Notification$Builder;

    :goto_3
    return-object v1
.end method

.method protected static buildNotificationIntent(Landroid/content/Context;Landroid/content/Intent;Landroid/app/PendingIntent;)Landroid/content/Intent;
    .locals 8

    .line 269
    invoke-virtual {p1}, Landroid/content/Intent;->clone()Ljava/lang/Object;

    move-result-object v0

    check-cast v0, Landroid/content/Intent;

    const-string v1, "tapIntent"

    .line 270
    invoke-virtual {v0, v1, p2}, Landroid/content/Intent;->putExtra(Ljava/lang/String;Landroid/os/Parcelable;)Landroid/content/Intent;

    const-string p2, "UNITY_STORED_NOTIFICATION_IDS"

    const/4 v1, 0x0

    .line 272
    invoke-virtual {p0, p2, v1}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object p2

    .line 273
    new-instance v2, Ljava/util/HashSet;

    new-instance v3, Ljava/util/HashSet;

    invoke-direct {v3}, Ljava/util/HashSet;-><init>()V

    const-string v4, "UNITY_NOTIFICATION_IDS"

    invoke-interface {p2, v4, v3}, Landroid/content/SharedPreferences;->getStringSet(Ljava/lang/String;Ljava/util/Set;)Ljava/util/Set;

    move-result-object v3

    invoke-direct {v2, v3}, Ljava/util/HashSet;-><init>(Ljava/util/Collection;)V

    .line 275
    new-instance v3, Ljava/util/HashSet;

    invoke-direct {v3}, Ljava/util/HashSet;-><init>()V

    .line 276
    invoke-interface {v2}, Ljava/util/Set;->iterator()Ljava/util/Iterator;

    move-result-object v2

    :cond_0
    :goto_0
    invoke-interface {v2}, Ljava/util/Iterator;->hasNext()Z

    move-result v5

    if-eqz v5, :cond_1

    invoke-interface {v2}, Ljava/util/Iterator;->next()Ljava/lang/Object;

    move-result-object v5

    check-cast v5, Ljava/lang/String;

    .line 279
    invoke-static {v5}, Ljava/lang/Integer;->valueOf(Ljava/lang/String;)Ljava/lang/Integer;

    move-result-object v6

    invoke-virtual {v6}, Ljava/lang/Integer;->intValue()I

    move-result v6

    const/high16 v7, 0x20000000

    invoke-static {p0, v6, p1, v7}, Lcom/unity/androidnotifications/UnityNotificationManager;->getBroadcastPendingIntent(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object v6

    if-eqz v6, :cond_0

    .line 282
    invoke-interface {v3, v5}, Ljava/util/Set;->add(Ljava/lang/Object;)Z

    goto :goto_0

    .line 286
    :cond_1
    sget-object p0, Landroid/os/Build;->MANUFACTURER:Ljava/lang/String;

    const-string p1, "samsung"

    invoke-virtual {p0, p1}, Ljava/lang/String;->equals(Ljava/lang/Object;)Z

    move-result p0

    if-eqz p0, :cond_2

    invoke-interface {v3}, Ljava/util/Set;->size()I

    move-result p0

    const/16 p1, 0x1f3

    if-lt p0, p1, :cond_2

    const-string p0, "UnityNotifications"

    const-string p1, "Attempting to schedule more than 500 notifications. There is a limit of 500 concurrently scheduled Alarms on Samsung devices either wait for the currently scheduled ones to be triggered or cancel them if you wish to schedule additional notifications."

    .line 289
    invoke-static {p0, p1}, Landroid/util/Log;->w(Ljava/lang/String;Ljava/lang/String;)I

    const/4 v0, 0x0

    goto :goto_1

    :cond_2
    const-string p0, "id"

    .line 293
    invoke-virtual {v0, p0, v1}, Landroid/content/Intent;->getIntExtra(Ljava/lang/String;I)I

    move-result p0

    .line 294
    invoke-static {p0}, Ljava/lang/Integer;->toString(I)Ljava/lang/String;

    move-result-object p0

    invoke-interface {v3, p0}, Ljava/util/Set;->add(Ljava/lang/Object;)Z

    const p0, 0x10008000

    .line 295
    invoke-virtual {v0, p0}, Landroid/content/Intent;->setFlags(I)Landroid/content/Intent;

    .line 298
    :goto_1
    invoke-interface {p2}, Landroid/content/SharedPreferences;->edit()Landroid/content/SharedPreferences$Editor;

    move-result-object p0

    invoke-interface {p0}, Landroid/content/SharedPreferences$Editor;->clear()Landroid/content/SharedPreferences$Editor;

    move-result-object p0

    .line 299
    invoke-interface {p0, v4, v3}, Landroid/content/SharedPreferences$Editor;->putStringSet(Ljava/lang/String;Ljava/util/Set;)Landroid/content/SharedPreferences$Editor;

    .line 300
    invoke-interface {p0}, Landroid/content/SharedPreferences$Editor;->apply()V

    return-object v0
.end method

.method protected static buildOpenAppIntent(Landroid/content/Intent;Landroid/content/Context;Ljava/lang/Class;)Landroid/content/Intent;
    .locals 1

    .line 260
    new-instance v0, Landroid/content/Intent;

    invoke-direct {v0, p1, p2}, Landroid/content/Intent;-><init>(Landroid/content/Context;Ljava/lang/Class;)V

    const/high16 p1, 0x30000000

    .line 261
    invoke-virtual {v0, p1}, Landroid/content/Intent;->addFlags(I)Landroid/content/Intent;

    .line 262
    invoke-virtual {v0, p0}, Landroid/content/Intent;->putExtras(Landroid/content/Intent;)Landroid/content/Intent;

    return-object v0
.end method

.method protected static cancelPendingNotificationIntent(Landroid/content/Context;I)V
    .locals 3

    .line 449
    new-instance v0, Landroid/content/Intent;

    const-class v1, Lcom/unity/androidnotifications/UnityNotificationManager;

    invoke-direct {v0, p0, v1}, Landroid/content/Intent;-><init>(Landroid/content/Context;Ljava/lang/Class;)V

    const/high16 v1, 0x20000000

    .line 450
    invoke-static {p0, p1, v0, v1}, Lcom/unity/androidnotifications/UnityNotificationManager;->getBroadcastPendingIntent(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object v0

    if-eqz v0, :cond_1

    if-eqz p0, :cond_0

    const-string v1, "alarm"

    .line 454
    invoke-virtual {p0, v1}, Landroid/content/Context;->getSystemService(Ljava/lang/String;)Ljava/lang/Object;

    move-result-object v1

    check-cast v1, Landroid/app/AlarmManager;

    .line 455
    invoke-virtual {v1, v0}, Landroid/app/AlarmManager;->cancel(Landroid/app/PendingIntent;)V

    .line 457
    :cond_0
    invoke-virtual {v0}, Landroid/app/PendingIntent;->cancel()V

    :cond_1
    const/4 v0, 0x0

    const-string v1, "UNITY_STORED_NOTIFICATION_IDS"

    .line 460
    invoke-virtual {p0, v1, v0}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object p0

    .line 461
    new-instance v0, Ljava/util/HashSet;

    new-instance v1, Ljava/util/HashSet;

    invoke-direct {v1}, Ljava/util/HashSet;-><init>()V

    const-string v2, "UNITY_NOTIFICATION_IDS"

    invoke-interface {p0, v2, v1}, Landroid/content/SharedPreferences;->getStringSet(Ljava/lang/String;Ljava/util/Set;)Ljava/util/Set;

    move-result-object v1

    invoke-direct {v0, v1}, Ljava/util/HashSet;-><init>(Ljava/util/Collection;)V

    .line 463
    invoke-static {p1}, Ljava/lang/Integer;->toString(I)Ljava/lang/String;

    move-result-object v1

    .line 464
    invoke-interface {v0, v1}, Ljava/util/Set;->contains(Ljava/lang/Object;)Z

    move-result v1

    if-eqz v1, :cond_2

    .line 465
    invoke-static {p1}, Ljava/lang/Integer;->toString(I)Ljava/lang/String;

    move-result-object p1

    invoke-interface {v0, p1}, Ljava/util/Set;->remove(Ljava/lang/Object;)Z

    .line 467
    invoke-interface {p0}, Landroid/content/SharedPreferences;->edit()Landroid/content/SharedPreferences$Editor;

    move-result-object p0

    invoke-interface {p0}, Landroid/content/SharedPreferences$Editor;->clear()Landroid/content/SharedPreferences$Editor;

    move-result-object p0

    .line 468
    invoke-interface {p0, v2, v0}, Landroid/content/SharedPreferences$Editor;->putStringSet(Ljava/lang/String;Ljava/util/Set;)Landroid/content/SharedPreferences$Editor;

    .line 469
    invoke-interface {p0}, Landroid/content/SharedPreferences$Editor;->apply()V

    :cond_2
    return-void
.end method

.method protected static deleteExpiredNotificationIntent(Landroid/content/Context;Ljava/lang/String;)V
    .locals 5

    const-string v0, "UNITY_STORED_NOTIFICATION_IDS"

    const/4 v1, 0x0

    .line 475
    invoke-virtual {p0, v0, v1}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object v0

    .line 476
    new-instance v2, Ljava/util/HashSet;

    new-instance v3, Ljava/util/HashSet;

    invoke-direct {v3}, Ljava/util/HashSet;-><init>()V

    const-string v4, "UNITY_NOTIFICATION_IDS"

    invoke-interface {v0, v4, v3}, Landroid/content/SharedPreferences;->getStringSet(Ljava/lang/String;Ljava/util/Set;)Ljava/util/Set;

    move-result-object v3

    invoke-direct {v2, v3}, Ljava/util/HashSet;-><init>(Ljava/util/Collection;)V

    .line 478
    invoke-static {p1}, Ljava/lang/Integer;->valueOf(Ljava/lang/String;)Ljava/lang/Integer;

    move-result-object v3

    invoke-virtual {v3}, Ljava/lang/Integer;->intValue()I

    move-result v3

    invoke-static {p0, v3}, Lcom/unity/androidnotifications/UnityNotificationManager;->cancelPendingNotificationIntent(Landroid/content/Context;I)V

    .line 480
    invoke-interface {v2, p1}, Ljava/util/Set;->remove(Ljava/lang/Object;)Z

    .line 481
    invoke-interface {v0}, Landroid/content/SharedPreferences;->edit()Landroid/content/SharedPreferences$Editor;

    move-result-object v0

    .line 482
    invoke-interface {v0, v4, v2}, Landroid/content/SharedPreferences$Editor;->putStringSet(Ljava/lang/String;Ljava/util/Set;)Landroid/content/SharedPreferences$Editor;

    .line 483
    invoke-interface {v0}, Landroid/content/SharedPreferences$Editor;->apply()V

    .line 485
    invoke-static {p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->getSharedPrefsNameByNotificationId(Ljava/lang/String;)Ljava/lang/String;

    move-result-object p1

    invoke-virtual {p0, p1, v1}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object p0

    .line 486
    invoke-interface {p0}, Landroid/content/SharedPreferences;->edit()Landroid/content/SharedPreferences$Editor;

    move-result-object p0

    invoke-interface {p0}, Landroid/content/SharedPreferences$Editor;->clear()Landroid/content/SharedPreferences$Editor;

    move-result-object p0

    invoke-interface {p0}, Landroid/content/SharedPreferences$Editor;->apply()V

    return-void
.end method

.method public static getActivityPendingIntent(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;
    .locals 2

    .line 306
    sget v0, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 v1, 0x17

    if-lt v0, v1, :cond_0

    const/high16 v0, 0x4000000

    or-int/2addr p3, v0

    .line 307
    invoke-static {p0, p1, p2, p3}, Landroid/app/PendingIntent;->getActivity(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object p0

    return-object p0

    .line 309
    :cond_0
    invoke-static {p0, p1, p2, p3}, Landroid/app/PendingIntent;->getActivity(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object p0

    return-object p0
.end method

.method public static getBroadcastPendingIntent(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;
    .locals 2

    .line 313
    sget v0, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 v1, 0x17

    if-lt v0, v1, :cond_0

    const/high16 v0, 0x4000000

    or-int/2addr p3, v0

    .line 314
    invoke-static {p0, p1, p2, p3}, Landroid/app/PendingIntent;->getBroadcast(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object p0

    return-object p0

    .line 316
    :cond_0
    invoke-static {p0, p1, p2, p3}, Landroid/app/PendingIntent;->getBroadcast(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object p0

    return-object p0
.end method

.method protected static getNotificationChannel(Landroid/content/Context;Ljava/lang/String;)Lcom/unity/androidnotifications/NotificationChannelWrapper;
    .locals 6

    .line 162
    sget v0, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 v1, 0x1a

    if-lt v0, v1, :cond_0

    .line 163
    invoke-static {p0, p1}, Lcom/unity/androidnotifications/UnityNotificationManagerOreo;->getOreoNotificationChannel(Landroid/content/Context;Ljava/lang/String;)Lcom/unity/androidnotifications/NotificationChannelWrapper;

    move-result-object p0

    return-object p0

    .line 166
    :cond_0
    invoke-static {p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->getSharedPrefsNameByChannelId(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v0

    const/4 v1, 0x0

    invoke-virtual {p0, v0, v1}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object p0

    .line 167
    new-instance v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;

    invoke-direct {v0}, Lcom/unity/androidnotifications/NotificationChannelWrapper;-><init>()V

    .line 169
    iput-object p1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->id:Ljava/lang/String;

    const-string p1, "title"

    const-string v2, "undefined"

    .line 170
    invoke-interface {p0, p1, v2}, Landroid/content/SharedPreferences;->getString(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;

    move-result-object p1

    iput-object p1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->name:Ljava/lang/String;

    const/4 p1, 0x3

    const-string v3, "importance"

    .line 171
    invoke-interface {p0, v3, p1}, Landroid/content/SharedPreferences;->getInt(Ljava/lang/String;I)I

    move-result p1

    iput p1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->importance:I

    const-string p1, "description"

    .line 172
    invoke-interface {p0, p1, v2}, Landroid/content/SharedPreferences;->getString(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;

    move-result-object p1

    iput-object p1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->description:Ljava/lang/String;

    const-string p1, "enableLights"

    .line 173
    invoke-interface {p0, p1, v1}, Landroid/content/SharedPreferences;->getBoolean(Ljava/lang/String;Z)Z

    move-result p1

    iput-boolean p1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->enableLights:Z

    const-string p1, "enableVibration"

    .line 174
    invoke-interface {p0, p1, v1}, Landroid/content/SharedPreferences;->getBoolean(Ljava/lang/String;Z)Z

    move-result p1

    iput-boolean p1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->enableVibration:Z

    const-string p1, "canBypassDnd"

    .line 175
    invoke-interface {p0, p1, v1}, Landroid/content/SharedPreferences;->getBoolean(Ljava/lang/String;Z)Z

    move-result p1

    iput-boolean p1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->canBypassDnd:Z

    const-string p1, "canShowBadge"

    .line 176
    invoke-interface {p0, p1, v1}, Landroid/content/SharedPreferences;->getBoolean(Ljava/lang/String;Z)Z

    move-result p1

    iput-boolean p1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->canShowBadge:Z

    const-string p1, "lockscreenVisibility"

    const/4 v2, 0x1

    .line 177
    invoke-interface {p0, p1, v2}, Landroid/content/SharedPreferences;->getInt(Ljava/lang/String;I)I

    move-result p1

    iput p1, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->lockscreenVisibility:I

    const-string p1, "vibrationPattern"

    const-string v3, "[]"

    .line 178
    invoke-interface {p0, p1, v3}, Landroid/content/SharedPreferences;->getString(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;

    move-result-object p0

    const-string p1, ","

    invoke-virtual {p0, p1}, Ljava/lang/String;->split(Ljava/lang/String;)[Ljava/lang/String;

    move-result-object p0

    .line 180
    array-length p1, p0

    new-array v3, p1, [J

    if-le p1, v2, :cond_1

    .line 183
    :goto_0
    array-length v4, p0

    if-ge v1, v4, :cond_1

    .line 185
    :try_start_0
    aget-object v4, p0, v1

    invoke-static {v4}, Ljava/lang/Long;->parseLong(Ljava/lang/String;)J

    move-result-wide v4

    aput-wide v4, v3, v1
    :try_end_0
    .catch Ljava/lang/NumberFormatException; {:try_start_0 .. :try_end_0} :catch_0

    goto :goto_1

    :catch_0
    const-wide/16 v4, 0x1

    .line 187
    aput-wide v4, v3, v1

    :goto_1
    add-int/lit8 v1, v1, 0x1

    goto :goto_0

    :cond_1
    if-le p1, v2, :cond_2

    goto :goto_2

    :cond_2
    const/4 v3, 0x0

    .line 192
    :goto_2
    iput-object v3, v0, Lcom/unity/androidnotifications/NotificationChannelWrapper;->vibrationPattern:[J

    return-object v0
.end method

.method public static getNotificationManager(Landroid/content/Context;)Landroid/app/NotificationManager;
    .locals 1

    const-string v0, "notification"

    .line 107
    invoke-virtual {p0, v0}, Landroid/content/Context;->getSystemService(Ljava/lang/String;)Ljava/lang/Object;

    move-result-object p0

    check-cast p0, Landroid/app/NotificationManager;

    return-object p0
.end method

.method public static getNotificationManagerImpl(Landroid/content/Context;)Lcom/unity/androidnotifications/UnityNotificationManager;
    .locals 1

    .line 84
    move-object v0, p0

    check-cast v0, Landroid/app/Activity;

    invoke-static {p0, v0}, Lcom/unity/androidnotifications/UnityNotificationManager;->getNotificationManagerImpl(Landroid/content/Context;Landroid/app/Activity;)Lcom/unity/androidnotifications/UnityNotificationManager;

    move-result-object p0

    return-object p0
.end method

.method public static getNotificationManagerImpl(Landroid/content/Context;Landroid/app/Activity;)Lcom/unity/androidnotifications/UnityNotificationManager;
    .locals 2

    .line 89
    sget-object v0, Lcom/unity/androidnotifications/UnityNotificationManager;->mUnityNotificationManager:Lcom/unity/androidnotifications/UnityNotificationManager;

    if-eqz v0, :cond_0

    return-object v0

    .line 92
    :cond_0
    sget v0, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 v1, 0x1a

    if-lt v0, v1, :cond_1

    .line 93
    new-instance v0, Lcom/unity/androidnotifications/UnityNotificationManagerOreo;

    invoke-direct {v0, p0, p1}, Lcom/unity/androidnotifications/UnityNotificationManagerOreo;-><init>(Landroid/content/Context;Landroid/app/Activity;)V

    sput-object v0, Lcom/unity/androidnotifications/UnityNotificationManager;->mUnityNotificationManager:Lcom/unity/androidnotifications/UnityNotificationManager;

    goto :goto_0

    .line 95
    :cond_1
    new-instance v0, Lcom/unity/androidnotifications/UnityNotificationManager;

    invoke-direct {v0, p0, p1}, Lcom/unity/androidnotifications/UnityNotificationManager;-><init>(Landroid/content/Context;Landroid/app/Activity;)V

    sput-object v0, Lcom/unity/androidnotifications/UnityNotificationManager;->mUnityNotificationManager:Lcom/unity/androidnotifications/UnityNotificationManager;

    .line 98
    :goto_0
    sget-object p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mUnityNotificationManager:Lcom/unity/androidnotifications/UnityNotificationManager;

    return-object p0
.end method

.method protected static getSharedPrefsNameByChannelId(Ljava/lang/String;)Ljava/lang/String;
    .locals 2

    const/4 v0, 0x1

    new-array v0, v0, [Ljava/lang/Object;

    const/4 v1, 0x0

    aput-object p0, v0, v1

    const-string p0, "unity_notification_channel_%s"

    .line 156
    invoke-static {p0, v0}, Ljava/lang/String;->format(Ljava/lang/String;[Ljava/lang/Object;)Ljava/lang/String;

    move-result-object p0

    return-object p0
.end method

.method protected static getSharedPrefsNameByNotificationId(Ljava/lang/String;)Ljava/lang/String;
    .locals 2

    const/4 v0, 0x1

    new-array v0, v0, [Ljava/lang/Object;

    const/4 v1, 0x0

    aput-object p0, v0, v1

    const-string p0, "u_notification_data_%s"

    .line 345
    invoke-static {p0, v0}, Ljava/lang/String;->format(Ljava/lang/String;[Ljava/lang/Object;)Ljava/lang/String;

    move-result-object p0

    return-object p0
.end method

.method protected static loadNotificationIntents(Landroid/content/Context;)Ljava/util/List;
    .locals 8
    .annotation system Ldalvik/annotation/Signature;
        value = {
            "(",
            "Landroid/content/Context;",
            ")",
            "Ljava/util/List<",
            "Landroid/content/Intent;",
            ">;"
        }
    .end annotation

    const-string v0, "UNITY_STORED_NOTIFICATION_IDS"

    const/4 v1, 0x0

    .line 350
    invoke-virtual {p0, v0, v1}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object v0

    .line 351
    new-instance v2, Ljava/util/HashSet;

    new-instance v3, Ljava/util/HashSet;

    invoke-direct {v3}, Ljava/util/HashSet;-><init>()V

    const-string v4, "UNITY_NOTIFICATION_IDS"

    invoke-interface {v0, v4, v3}, Landroid/content/SharedPreferences;->getStringSet(Ljava/lang/String;Ljava/util/Set;)Ljava/util/Set;

    move-result-object v0

    invoke-direct {v2, v0}, Ljava/util/HashSet;-><init>(Ljava/util/Collection;)V

    .line 353
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    .line 354
    new-instance v3, Ljava/util/HashSet;

    invoke-direct {v3}, Ljava/util/HashSet;-><init>()V

    .line 356
    invoke-interface {v2}, Ljava/util/Set;->iterator()Ljava/util/Iterator;

    move-result-object v2

    :goto_0
    invoke-interface {v2}, Ljava/util/Iterator;->hasNext()Z

    move-result v4

    if-eqz v4, :cond_1

    invoke-interface {v2}, Ljava/util/Iterator;->next()Ljava/lang/Object;

    move-result-object v4

    check-cast v4, Ljava/lang/String;

    .line 357
    invoke-static {v4}, Lcom/unity/androidnotifications/UnityNotificationManager;->getSharedPrefsNameByNotificationId(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v5

    invoke-virtual {p0, v5, v1}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object v5

    const-string v6, "data"

    const-string v7, ""

    .line 358
    invoke-interface {v5, v6, v7}, Landroid/content/SharedPreferences;->getString(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/String;

    move-result-object v5

    .line 360
    invoke-virtual {v5}, Ljava/lang/String;->length()I

    move-result v6

    const/4 v7, 0x1

    if-le v6, v7, :cond_0

    .line 361
    invoke-static {p0, v5}, Lcom/unity/androidnotifications/UnityNotificationUtilities;->deserializeNotificationIntent(Landroid/content/Context;Ljava/lang/String;)Landroid/content/Intent;

    move-result-object v4

    .line 362
    invoke-interface {v0, v4}, Ljava/util/List;->add(Ljava/lang/Object;)Z

    goto :goto_0

    .line 364
    :cond_0
    invoke-interface {v3, v4}, Ljava/util/Set;->add(Ljava/lang/Object;)Z

    goto :goto_0

    .line 368
    :cond_1
    invoke-interface {v3}, Ljava/util/Set;->iterator()Ljava/util/Iterator;

    move-result-object v1

    :goto_1
    invoke-interface {v1}, Ljava/util/Iterator;->hasNext()Z

    move-result v2

    if-eqz v2, :cond_2

    invoke-interface {v1}, Ljava/util/Iterator;->next()Ljava/lang/Object;

    move-result-object v2

    check-cast v2, Ljava/lang/String;

    .line 369
    invoke-static {p0, v2}, Lcom/unity/androidnotifications/UnityNotificationManager;->deleteExpiredNotificationIntent(Landroid/content/Context;Ljava/lang/String;)V

    goto :goto_1

    :cond_2
    return-object v0
.end method

.method protected static notify(Landroid/content/Context;ILandroid/app/Notification;Landroid/content/Intent;)V
    .locals 3

    .line 646
    invoke-static {p0}, Lcom/unity/androidnotifications/UnityNotificationManager;->getNotificationManager(Landroid/content/Context;)Landroid/app/NotificationManager;

    move-result-object v0

    invoke-virtual {v0, p1, p2}, Landroid/app/NotificationManager;->notify(ILandroid/app/Notification;)V

    .line 649
    :try_start_0
    sget-object p2, Lcom/unity/androidnotifications/UnityNotificationManager;->mNotificationCallback:Lcom/unity/androidnotifications/NotificationCallback;

    invoke-interface {p2, p3}, Lcom/unity/androidnotifications/NotificationCallback;->onSentNotification(Landroid/content/Intent;)V
    :try_end_0
    .catch Ljava/lang/RuntimeException; {:try_start_0 .. :try_end_0} :catch_0

    goto :goto_0

    :catch_0
    const-string p2, "UnityNotifications"

    const-string v0, "Can not invoke OnNotificationReceived event when the app is not running!"

    .line 651
    invoke-static {p2, v0}, Landroid/util/Log;->w(Ljava/lang/String;Ljava/lang/String;)I

    :goto_0
    const-string p2, "repeatInterval"

    const-wide/16 v0, 0x0

    .line 654
    invoke-virtual {p3, p2, v0, v1}, Landroid/content/Intent;->getLongExtra(Ljava/lang/String;J)J

    move-result-wide p2

    cmp-long v2, p2, v0

    if-lez v2, :cond_0

    const/4 p2, 0x1

    goto :goto_1

    :cond_0
    const/4 p2, 0x0

    :goto_1
    if-nez p2, :cond_1

    .line 657
    invoke-static {p1}, Ljava/lang/Integer;->toString(I)Ljava/lang/String;

    move-result-object p1

    invoke-static {p0, p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->deleteExpiredNotificationIntent(Landroid/content/Context;Ljava/lang/String;)V

    :cond_1
    return-void
.end method

.method protected static saveNotificationIntent(Landroid/content/Context;Landroid/content/Intent;)V
    .locals 4

    const-string v0, "id"

    const/4 v1, 0x0

    .line 322
    invoke-virtual {p1, v0, v1}, Landroid/content/Intent;->getIntExtra(Ljava/lang/String;I)I

    move-result v0

    invoke-static {v0}, Ljava/lang/Integer;->toString(I)Ljava/lang/String;

    move-result-object v0

    .line 323
    invoke-static {v0}, Lcom/unity/androidnotifications/UnityNotificationManager;->getSharedPrefsNameByNotificationId(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v2

    invoke-virtual {p0, v2, v1}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object v2

    .line 325
    invoke-interface {v2}, Landroid/content/SharedPreferences;->edit()Landroid/content/SharedPreferences$Editor;

    move-result-object v2

    invoke-interface {v2}, Landroid/content/SharedPreferences$Editor;->clear()Landroid/content/SharedPreferences$Editor;

    move-result-object v2

    .line 326
    invoke-static {p1}, Lcom/unity/androidnotifications/UnityNotificationUtilities;->serializeNotificationIntent(Landroid/content/Intent;)Ljava/lang/String;

    move-result-object p1

    const-string v3, "data"

    .line 327
    invoke-interface {v2, v3, p1}, Landroid/content/SharedPreferences$Editor;->putString(Ljava/lang/String;Ljava/lang/String;)Landroid/content/SharedPreferences$Editor;

    .line 328
    invoke-interface {v2}, Landroid/content/SharedPreferences$Editor;->apply()V

    const-string p1, "UNITY_STORED_NOTIFICATION_IDS"

    .line 331
    invoke-virtual {p0, p1, v1}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object p1

    .line 332
    new-instance v1, Ljava/util/HashSet;

    new-instance v2, Ljava/util/HashSet;

    invoke-direct {v2}, Ljava/util/HashSet;-><init>()V

    const-string v3, "UNITY_NOTIFICATION_IDS"

    invoke-interface {p1, v3, v2}, Landroid/content/SharedPreferences;->getStringSet(Ljava/lang/String;Ljava/util/Set;)Ljava/util/Set;

    move-result-object v2

    invoke-direct {v1, v2}, Ljava/util/HashSet;-><init>(Ljava/util/Collection;)V

    .line 333
    invoke-interface {v1, v0}, Ljava/util/Set;->add(Ljava/lang/Object;)Z

    .line 335
    invoke-interface {p1}, Landroid/content/SharedPreferences;->edit()Landroid/content/SharedPreferences$Editor;

    move-result-object p1

    invoke-interface {p1}, Landroid/content/SharedPreferences$Editor;->clear()Landroid/content/SharedPreferences$Editor;

    move-result-object p1

    .line 336
    invoke-interface {p1, v3, v1}, Landroid/content/SharedPreferences$Editor;->putStringSet(Ljava/lang/String;Ljava/util/Set;)Landroid/content/SharedPreferences$Editor;

    .line 337
    invoke-interface {p1}, Landroid/content/SharedPreferences$Editor;->apply()V

    .line 340
    invoke-static {p0}, Lcom/unity/androidnotifications/UnityNotificationManager;->loadNotificationIntents(Landroid/content/Context;)Ljava/util/List;

    return-void
.end method

.method protected static scheduleNotificationIntentAlarm(Landroid/content/Context;Landroid/content/Intent;Landroid/app/PendingIntent;)V
    .locals 10

    const-string v0, "repeatInterval"

    const-wide/16 v1, 0x0

    .line 377
    invoke-virtual {p1, v0, v1, v2}, Landroid/content/Intent;->getLongExtra(Ljava/lang/String;J)J

    move-result-wide v7

    const-string v0, "fireTime"

    .line 378
    invoke-virtual {p1, v0, v1, v2}, Landroid/content/Intent;->getLongExtra(Ljava/lang/String;J)J

    move-result-wide v5

    const-string p1, "alarm"

    .line 380
    invoke-virtual {p0, p1}, Landroid/content/Context;->getSystemService(Ljava/lang/String;)Ljava/lang/Object;

    move-result-object p0

    move-object v3, p0

    check-cast v3, Landroid/app/AlarmManager;

    cmp-long p0, v7, v1

    if-gtz p0, :cond_1

    .line 383
    sget p0, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 p1, 0x17

    const/4 v0, 0x0

    if-lt p0, p1, :cond_0

    .line 384
    invoke-virtual {v3, v0, v5, v6, p2}, Landroid/app/AlarmManager;->setExactAndAllowWhileIdle(IJLandroid/app/PendingIntent;)V

    goto :goto_0

    .line 386
    :cond_0
    invoke-virtual {v3, v0, v5, v6, p2}, Landroid/app/AlarmManager;->set(IJLandroid/app/PendingIntent;)V

    goto :goto_0

    :cond_1
    const/4 v4, 0x0

    move-object v9, p2

    .line 389
    invoke-virtual/range {v3 .. v9}, Landroid/app/AlarmManager;->setInexactRepeating(IJJLandroid/app/PendingIntent;)V

    :goto_0
    return-void
.end method

.method protected static sendNotification(Landroid/content/Context;Landroid/content/Intent;)V
    .locals 3

    .line 513
    invoke-static {p0, p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->buildNotification(Landroid/content/Context;Landroid/content/Intent;)Landroid/app/Notification$Builder;

    move-result-object v0

    const-string v1, "id"

    const/4 v2, -0x1

    .line 514
    invoke-virtual {p1, v1, v2}, Landroid/content/Intent;->getIntExtra(Ljava/lang/String;I)I

    move-result v1

    .line 516
    invoke-virtual {v0}, Landroid/app/Notification$Builder;->build()Landroid/app/Notification;

    move-result-object v0

    invoke-static {p0, v1, v0, p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->notify(Landroid/content/Context;ILandroid/app/Notification;Landroid/content/Intent;)V

    return-void
.end method


# virtual methods
.method public cancelAllNotifications()V
    .locals 1

    .line 496
    invoke-virtual {p0}, Lcom/unity/androidnotifications/UnityNotificationManager;->getNotificationManager()Landroid/app/NotificationManager;

    move-result-object v0

    invoke-virtual {v0}, Landroid/app/NotificationManager;->cancelAll()V

    return-void
.end method

.method public cancelAllPendingNotificationIntents()V
    .locals 4

    .line 418
    invoke-virtual {p0}, Lcom/unity/androidnotifications/UnityNotificationManager;->getScheduledNotificationIDs()[I

    move-result-object v0

    .line 420
    array-length v1, v0

    const/4 v2, 0x0

    :goto_0
    if-ge v2, v1, :cond_0

    aget v3, v0, v2

    .line 421
    invoke-virtual {p0, v3}, Lcom/unity/androidnotifications/UnityNotificationManager;->cancelPendingNotificationIntent(I)V

    add-int/lit8 v2, v2, 0x1

    goto :goto_0

    :cond_0
    return-void
.end method

.method public cancelDisplayedNotification(I)V
    .locals 1

    .line 491
    invoke-virtual {p0}, Lcom/unity/androidnotifications/UnityNotificationManager;->getNotificationManager()Landroid/app/NotificationManager;

    move-result-object v0

    invoke-virtual {v0, p1}, Landroid/app/NotificationManager;->cancel(I)V

    return-void
.end method

.method public cancelPendingNotificationIntent(I)V
    .locals 1

    .line 441
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {v0, p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->cancelPendingNotificationIntent(Landroid/content/Context;I)V

    .line 442
    iget-boolean v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mRescheduleOnRestart:Z

    if-eqz v0, :cond_0

    .line 443
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {p1}, Ljava/lang/Integer;->toString(I)Ljava/lang/String;

    move-result-object p1

    invoke-static {v0, p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->deleteExpiredNotificationIntent(Landroid/content/Context;Ljava/lang/String;)V

    :cond_0
    return-void
.end method

.method public checkIfPendingNotificationIsRegistered(I)Z
    .locals 3

    .line 412
    new-instance v0, Landroid/content/Intent;

    iget-object v1, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mActivity:Landroid/app/Activity;

    const-class v2, Lcom/unity/androidnotifications/UnityNotificationManager;

    invoke-direct {v0, v1, v2}, Landroid/content/Intent;-><init>(Landroid/content/Context;Ljava/lang/Class;)V

    .line 413
    iget-object v1, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    const/high16 v2, 0x20000000

    invoke-static {v1, p1, v0, v2}, Lcom/unity/androidnotifications/UnityNotificationManager;->getBroadcastPendingIntent(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object p1

    if-eqz p1, :cond_0

    const/4 p1, 0x1

    goto :goto_0

    :cond_0
    const/4 p1, 0x0

    :goto_0
    return p1
.end method

.method public checkNotificationStatus(I)I
    .locals 5

    .line 395
    sget v0, Landroid/os/Build$VERSION;->SDK_INT:I

    const/16 v1, 0x17

    if-lt v0, v1, :cond_3

    .line 397
    invoke-virtual {p0}, Lcom/unity/androidnotifications/UnityNotificationManager;->getNotificationManager()Landroid/app/NotificationManager;

    move-result-object v0

    invoke-virtual {v0}, Landroid/app/NotificationManager;->getActiveNotifications()[Landroid/service/notification/StatusBarNotification;

    move-result-object v0

    array-length v1, v0

    const/4 v2, 0x0

    const/4 v3, 0x0

    :goto_0
    if-ge v3, v1, :cond_1

    aget-object v4, v0, v3

    .line 398
    invoke-virtual {v4}, Landroid/service/notification/StatusBarNotification;->getId()I

    move-result v4

    if-ne p1, v4, :cond_0

    const/4 p1, 0x2

    return p1

    :cond_0
    add-int/lit8 v3, v3, 0x1

    goto :goto_0

    .line 402
    :cond_1
    invoke-virtual {p0, p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->checkIfPendingNotificationIsRegistered(I)Z

    move-result p1

    if-eqz p1, :cond_2

    const/4 p1, 0x1

    return p1

    :cond_2
    return v2

    :cond_3
    const/4 p1, -0x1

    return p1
.end method

.method public deleteNotificationChannel(Ljava/lang/String;)V
    .locals 5

    .line 205
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    const-string v1, "UNITY_NOTIFICATIONS"

    const/4 v2, 0x0

    invoke-virtual {v0, v1, v2}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object v0

    .line 206
    new-instance v1, Ljava/util/HashSet;

    new-instance v3, Ljava/util/HashSet;

    invoke-direct {v3}, Ljava/util/HashSet;-><init>()V

    const-string v4, "ChannelIDs"

    invoke-interface {v0, v4, v3}, Landroid/content/SharedPreferences;->getStringSet(Ljava/lang/String;Ljava/util/Set;)Ljava/util/Set;

    move-result-object v3

    invoke-direct {v1, v3}, Ljava/util/HashSet;-><init>(Ljava/util/Collection;)V

    .line 208
    invoke-interface {v1, p1}, Ljava/util/Set;->contains(Ljava/lang/Object;)Z

    move-result v3

    if-nez v3, :cond_0

    return-void

    .line 212
    :cond_0
    invoke-interface {v1, p1}, Ljava/util/Set;->remove(Ljava/lang/Object;)Z

    .line 213
    invoke-interface {v0}, Landroid/content/SharedPreferences;->edit()Landroid/content/SharedPreferences$Editor;

    move-result-object v0

    invoke-interface {v0}, Landroid/content/SharedPreferences$Editor;->clear()Landroid/content/SharedPreferences$Editor;

    move-result-object v0

    .line 214
    invoke-interface {v0, v4, v1}, Landroid/content/SharedPreferences$Editor;->putStringSet(Ljava/lang/String;Ljava/util/Set;)Landroid/content/SharedPreferences$Editor;

    .line 215
    invoke-interface {v0}, Landroid/content/SharedPreferences$Editor;->apply()V

    .line 218
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->getSharedPrefsNameByChannelId(Ljava/lang/String;)Ljava/lang/String;

    move-result-object p1

    invoke-virtual {v0, p1, v2}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object p1

    .line 219
    invoke-interface {p1}, Landroid/content/SharedPreferences;->edit()Landroid/content/SharedPreferences$Editor;

    move-result-object p1

    invoke-interface {p1}, Landroid/content/SharedPreferences$Editor;->clear()Landroid/content/SharedPreferences$Editor;

    move-result-object p1

    invoke-interface {p1}, Landroid/content/SharedPreferences$Editor;->apply()V

    return-void
.end method

.method protected getNotificationChannel(Ljava/lang/String;)Lcom/unity/androidnotifications/NotificationChannelWrapper;
    .locals 1

    .line 199
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {v0, p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->getNotificationChannel(Landroid/content/Context;Ljava/lang/String;)Lcom/unity/androidnotifications/NotificationChannelWrapper;

    move-result-object p1

    return-object p1
.end method

.method public getNotificationChannels()[Ljava/lang/Object;
    .locals 3

    .line 225
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    const-string v1, "UNITY_NOTIFICATIONS"

    const/4 v2, 0x0

    invoke-virtual {v0, v1, v2}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object v0

    .line 226
    new-instance v1, Ljava/util/HashSet;

    invoke-direct {v1}, Ljava/util/HashSet;-><init>()V

    const-string v2, "ChannelIDs"

    invoke-interface {v0, v2, v1}, Landroid/content/SharedPreferences;->getStringSet(Ljava/lang/String;Ljava/util/Set;)Ljava/util/Set;

    move-result-object v0

    .line 228
    new-instance v1, Ljava/util/ArrayList;

    invoke-direct {v1}, Ljava/util/ArrayList;-><init>()V

    .line 230
    invoke-interface {v0}, Ljava/util/Set;->iterator()Ljava/util/Iterator;

    move-result-object v0

    :goto_0
    invoke-interface {v0}, Ljava/util/Iterator;->hasNext()Z

    move-result v2

    if-eqz v2, :cond_0

    invoke-interface {v0}, Ljava/util/Iterator;->next()Ljava/lang/Object;

    move-result-object v2

    check-cast v2, Ljava/lang/String;

    .line 231
    invoke-virtual {p0, v2}, Lcom/unity/androidnotifications/UnityNotificationManager;->getNotificationChannel(Ljava/lang/String;)Lcom/unity/androidnotifications/NotificationChannelWrapper;

    move-result-object v2

    invoke-virtual {v1, v2}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    goto :goto_0

    .line 233
    :cond_0
    invoke-virtual {v1}, Ljava/util/ArrayList;->toArray()[Ljava/lang/Object;

    move-result-object v0

    return-object v0
.end method

.method public getNotificationManager()Landroid/app/NotificationManager;
    .locals 1

    .line 102
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {v0}, Lcom/unity/androidnotifications/UnityNotificationManager;->getNotificationManager(Landroid/content/Context;)Landroid/app/NotificationManager;

    move-result-object v0

    return-object v0
.end method

.method protected getScheduledNotificationIDs()[I
    .locals 5

    .line 427
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    const-string v1, "UNITY_STORED_NOTIFICATION_IDS"

    const/4 v2, 0x0

    invoke-virtual {v0, v1, v2}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object v0

    .line 428
    new-instance v1, Ljava/util/HashSet;

    invoke-direct {v1}, Ljava/util/HashSet;-><init>()V

    const-string v3, "UNITY_NOTIFICATION_IDS"

    invoke-interface {v0, v3, v1}, Landroid/content/SharedPreferences;->getStringSet(Ljava/lang/String;Ljava/util/Set;)Ljava/util/Set;

    move-result-object v0

    .line 431
    invoke-interface {v0}, Ljava/util/Set;->size()I

    move-result v1

    new-array v1, v1, [I

    .line 433
    invoke-interface {v0}, Ljava/util/Set;->iterator()Ljava/util/Iterator;

    move-result-object v0

    :goto_0
    invoke-interface {v0}, Ljava/util/Iterator;->hasNext()Z

    move-result v3

    if-eqz v3, :cond_0

    invoke-interface {v0}, Ljava/util/Iterator;->next()Ljava/lang/Object;

    move-result-object v3

    check-cast v3, Ljava/lang/String;

    add-int/lit8 v4, v2, 0x1

    .line 434
    invoke-static {v3}, Ljava/lang/Integer;->valueOf(Ljava/lang/String;)Ljava/lang/Integer;

    move-result-object v3

    invoke-virtual {v3}, Ljava/lang/Integer;->intValue()I

    move-result v3

    aput v3, v1, v2

    move v2, v4

    goto :goto_0

    :cond_0
    return-object v1
.end method

.method public onReceive(Landroid/content/Context;Landroid/content/Intent;)V
    .locals 1

    :try_start_0
    const-string v0, "channelID"

    .line 502
    invoke-virtual {p2, v0}, Landroid/content/Intent;->hasExtra(Ljava/lang/String;)Z

    move-result v0

    if-eqz v0, :cond_1

    const-string v0, "smallIconStr"

    invoke-virtual {p2, v0}, Landroid/content/Intent;->hasExtra(Ljava/lang/String;)Z

    move-result v0

    if-nez v0, :cond_0

    goto :goto_0

    .line 505
    :cond_0
    invoke-static {p1, p2}, Lcom/unity/androidnotifications/UnityNotificationManager;->sendNotification(Landroid/content/Context;Landroid/content/Intent;)V
    :try_end_0
    .catch Landroid/os/BadParcelableException; {:try_start_0 .. :try_end_0} :catch_0

    goto :goto_1

    :cond_1
    :goto_0
    return-void

    :catch_0
    move-exception p1

    .line 507
    invoke-virtual {p1}, Landroid/os/BadParcelableException;->toString()Ljava/lang/String;

    move-result-object p1

    const-string p2, "UnityNotifications"

    invoke-static {p2, p1}, Landroid/util/Log;->w(Ljava/lang/String;Ljava/lang/String;)I

    :goto_1
    return-void
.end method

.method public registerNotificationChannel(Ljava/lang/String;Ljava/lang/String;ILjava/lang/String;ZZZZ[JI)V
    .locals 5

    .line 128
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    const-string v1, "UNITY_NOTIFICATIONS"

    const/4 v2, 0x0

    invoke-virtual {v0, v1, v2}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object v0

    .line 129
    new-instance v1, Ljava/util/HashSet;

    new-instance v3, Ljava/util/HashSet;

    invoke-direct {v3}, Ljava/util/HashSet;-><init>()V

    const-string v4, "ChannelIDs"

    invoke-interface {v0, v4, v3}, Landroid/content/SharedPreferences;->getStringSet(Ljava/lang/String;Ljava/util/Set;)Ljava/util/Set;

    move-result-object v3

    invoke-direct {v1, v3}, Ljava/util/HashSet;-><init>(Ljava/util/Collection;)V

    .line 130
    invoke-interface {v1, p1}, Ljava/util/Set;->add(Ljava/lang/Object;)Z

    .line 133
    invoke-interface {v0}, Landroid/content/SharedPreferences;->edit()Landroid/content/SharedPreferences$Editor;

    move-result-object v0

    invoke-interface {v0}, Landroid/content/SharedPreferences$Editor;->clear()Landroid/content/SharedPreferences$Editor;

    move-result-object v0

    .line 134
    invoke-interface {v0, v4, v1}, Landroid/content/SharedPreferences$Editor;->putStringSet(Ljava/lang/String;Ljava/util/Set;)Landroid/content/SharedPreferences$Editor;

    .line 135
    invoke-interface {v0}, Landroid/content/SharedPreferences$Editor;->apply()V

    .line 138
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->getSharedPrefsNameByChannelId(Ljava/lang/String;)Ljava/lang/String;

    move-result-object p1

    invoke-virtual {v0, p1, v2}, Landroid/content/Context;->getSharedPreferences(Ljava/lang/String;I)Landroid/content/SharedPreferences;

    move-result-object p1

    .line 139
    invoke-interface {p1}, Landroid/content/SharedPreferences;->edit()Landroid/content/SharedPreferences$Editor;

    move-result-object p1

    const-string v0, "title"

    .line 141
    invoke-interface {p1, v0, p2}, Landroid/content/SharedPreferences$Editor;->putString(Ljava/lang/String;Ljava/lang/String;)Landroid/content/SharedPreferences$Editor;

    const-string p2, "importance"

    .line 142
    invoke-interface {p1, p2, p3}, Landroid/content/SharedPreferences$Editor;->putInt(Ljava/lang/String;I)Landroid/content/SharedPreferences$Editor;

    const-string p2, "description"

    .line 143
    invoke-interface {p1, p2, p4}, Landroid/content/SharedPreferences$Editor;->putString(Ljava/lang/String;Ljava/lang/String;)Landroid/content/SharedPreferences$Editor;

    const-string p2, "enableLights"

    .line 144
    invoke-interface {p1, p2, p5}, Landroid/content/SharedPreferences$Editor;->putBoolean(Ljava/lang/String;Z)Landroid/content/SharedPreferences$Editor;

    const-string p2, "enableVibration"

    .line 145
    invoke-interface {p1, p2, p6}, Landroid/content/SharedPreferences$Editor;->putBoolean(Ljava/lang/String;Z)Landroid/content/SharedPreferences$Editor;

    const-string p2, "canBypassDnd"

    .line 146
    invoke-interface {p1, p2, p7}, Landroid/content/SharedPreferences$Editor;->putBoolean(Ljava/lang/String;Z)Landroid/content/SharedPreferences$Editor;

    const-string p2, "canShowBadge"

    .line 147
    invoke-interface {p1, p2, p8}, Landroid/content/SharedPreferences$Editor;->putBoolean(Ljava/lang/String;Z)Landroid/content/SharedPreferences$Editor;

    .line 148
    invoke-static {p9}, Ljava/util/Arrays;->toString([J)Ljava/lang/String;

    move-result-object p2

    const-string p3, "vibrationPattern"

    invoke-interface {p1, p3, p2}, Landroid/content/SharedPreferences$Editor;->putString(Ljava/lang/String;Ljava/lang/String;)Landroid/content/SharedPreferences$Editor;

    const-string p2, "lockscreenVisibility"

    .line 149
    invoke-interface {p1, p2, p10}, Landroid/content/SharedPreferences$Editor;->putInt(Ljava/lang/String;I)Landroid/content/SharedPreferences$Editor;

    .line 151
    invoke-interface {p1}, Landroid/content/SharedPreferences$Editor;->apply()V

    return-void
.end method

.method public scheduleNotificationIntent(Landroid/content/Intent;)V
    .locals 4

    .line 239
    invoke-static {p1}, Lcom/unity/androidnotifications/UnityNotificationUtilities;->serializeNotificationIntent(Landroid/content/Intent;)Ljava/lang/String;

    move-result-object p1

    .line 240
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {v0, p1}, Lcom/unity/androidnotifications/UnityNotificationUtilities;->deserializeNotificationIntent(Landroid/content/Context;Ljava/lang/String;)Landroid/content/Intent;

    move-result-object p1

    const-string v0, "id"

    const/4 v1, 0x0

    .line 242
    invoke-virtual {p1, v0, v1}, Landroid/content/Intent;->getIntExtra(Ljava/lang/String;I)I

    move-result v0

    .line 244
    iget-object v2, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    iget-object v3, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mOpenActivity:Ljava/lang/Class;

    invoke-static {p1, v2, v3}, Lcom/unity/androidnotifications/UnityNotificationManager;->buildOpenAppIntent(Landroid/content/Intent;Landroid/content/Context;Ljava/lang/Class;)Landroid/content/Intent;

    move-result-object v2

    .line 245
    iget-object v3, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {v3, v0, v2, v1}, Lcom/unity/androidnotifications/UnityNotificationManager;->getActivityPendingIntent(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object v1

    .line 246
    iget-object v2, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {v2, p1, v1}, Lcom/unity/androidnotifications/UnityNotificationManager;->buildNotificationIntent(Landroid/content/Context;Landroid/content/Intent;Landroid/app/PendingIntent;)Landroid/content/Intent;

    move-result-object v1

    if-eqz v1, :cond_1

    .line 249
    iget-boolean v2, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mRescheduleOnRestart:Z

    if-eqz v2, :cond_0

    .line 250
    iget-object v2, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {v2, p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->saveNotificationIntent(Landroid/content/Context;Landroid/content/Intent;)V

    .line 253
    :cond_0
    iget-object p1, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    const/high16 v2, 0x8000000

    invoke-static {p1, v0, v1, v2}, Lcom/unity/androidnotifications/UnityNotificationManager;->getBroadcastPendingIntent(Landroid/content/Context;ILandroid/content/Intent;I)Landroid/app/PendingIntent;

    move-result-object p1

    .line 254
    iget-object v0, p0, Lcom/unity/androidnotifications/UnityNotificationManager;->mContext:Landroid/content/Context;

    invoke-static {v0, v1, p1}, Lcom/unity/androidnotifications/UnityNotificationManager;->scheduleNotificationIntentAlarm(Landroid/content/Context;Landroid/content/Intent;Landroid/app/PendingIntent;)V

    :cond_1
    return-void
.end method

.method public setNotificationCallback(Lcom/unity/androidnotifications/NotificationCallback;)V
    .locals 0

    .line 112
    sput-object p1, Lcom/unity/androidnotifications/UnityNotificationManager;->mNotificationCallback:Lcom/unity/androidnotifications/NotificationCallback;

    return-void
.end method
