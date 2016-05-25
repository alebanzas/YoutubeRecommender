WITH 
shortEmotions AS
(
    SELECT Id,
		 Emotion ,
         count(*) as [Count],
		max([Timestamp]) as [Timestamp],
		DeviceId
 FROM input timestamp by [Timestamp]
 where not Emotion is null
 GROUP BY TumblingWindow(second,30), Id, Emotion, DeviceId
 having count(*)>1
  ),
 longEmotions AS
 (
SELECT Id,
		 Emotion ,
         count(*) as [Count],
		max([Timestamp]) as [Timestamp]
 FROM input timestamp by [Timestamp]
 where not Emotion is null
 GROUP BY HoppingWindow(DURATION(ss,120), HOP(ss,90)), Id, Emotion, DeviceId
 ),
 compare AS
 (
     SELECT shortEmotions.Id AS Id, shortEmotions.Emotion, shortEmotions.[Count], shortEmotions.[Timestamp], shortEmotions.DeviceId
     FROM shortEmotions
     INNER JOIN longEmotions
      ON shortEmotions.Id=longEmotions.Id AND NOT shortEmotions.Emotion = longEmotions.Emotion
      AND DATEDIFF(ss, shortEmotions, longEmotions) BETWEEN 0 AND 30
 ),
 heartrateavg as
 (
    SELECT Id, avg(HeartRate) as HeartRate, max([Timestamp]) as [Timestamp], DeviceId
    from input timestamp by [Timestamp]
    where not HeartRate is null
    GROUP BY TumblingWindow(second,30),Id,DeviceId
 ),
 data as(
  SELECT compare.id as Id, compare.Emotion as Mood, heartrateavg.HeartRate, compare.[Timestamp] as [Timestamp], compare.DeviceId as DeviceId
 from compare
  LEFT OUTER JOIN heartrateavg ON compare.Id=heartrateavg.Id AND
   DATEDIFF(ss, compare, heartrateavg) BETWEEN -60 AND 60
)

select *, 'false' as Done  into storage from data
select *, 'false' as Done  into storagejuan from data
select * into storagetest from input timestamp by [Timestamp]
select * into pbi from data
--select count(*), max([Timestamp]) as [Timestamp] into storagetest from input timestamp by [Timestamp] group by tumblingwindow(second,30)
--select *, System.Timestamp as [Timestamp] into storagetest from input timestamp by [Timestamp]