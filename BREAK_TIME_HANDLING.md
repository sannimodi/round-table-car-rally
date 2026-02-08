# Break Time Handling: Single-Scan vs Dual-Scan Illustration

This document provides a detailed comparison between the old dual-scan method (2024 style) and the new single-scan method (2025 style) for handling break time at rally checkpoints.

## Rally Scenario Setup

**Rally Configuration:**
- LatePenalty = 2 points/min
- EarlyPenalty = 3 points/min
- ExtraBreakPenalty = 1 point/min
- RoundingThreshold = 30 seconds

**Route:**
```
START → CP1 (no break) → CP2 (15-min break) → CP3 (no break) → FINISH
        20 min            25 min                30 min
```

**Participant "Car #5" Timeline:**
```
START:  10:00:00 (departs on time)
CP1:    10:22:00 (arrives 2 min late, no break)
CP2:    10:49:00 (arrives 2 min late, takes 18 min break instead of 15)
CP3:    11:20:00 (arrives)
```

---

## 🔴 OLD METHOD (Dual-Scan) - 2024 Style

### CSV Data Format:
```csv
# marshals/CP2.001.csv
Car Code,Time Captured
ART40/25/005,10:49:00 | 11:07:00    ← Both entry AND exit scanned
```

### Checkpoint Calculations:

**CP1 (No Break):**
```
Expected arrival: 10:20:00 (10:00 + 20 min)
Actual arrival:   10:22:00
Departure:        10:22:00 (scanned)

Time difference: 2 min late
Penalty: 2 min × 2 (LatePenalty) = 4 points
```

**CP2 (15-min Break - DUAL SCAN):**
```
Expected arrival: 10:47:00 (10:22 + 25 min)
Actual arrival:   10:49:00 (first scan)
Actual departure: 11:07:00 (second scan)

Arrival penalty:
  Time difference: 2 min late
  Penalty: 2 min × 2 (LatePenalty) = 4 points

Break penalty:
  Actual break: 18 min (11:07 - 10:49)
  Allowed break: 15 min
  Overage: 3 min
  Penalty: 3 min × 1 (ExtraBreakPenalty) = 3 points

Total CP2 penalty: 4 + 3 = 7 points
```

**CP3 (No Break):**
```
Expected arrival: 11:37:00 (10:47 + 15 + 30 min)
Previous departure: 11:07:00 (actual scanned from CP2)
Actual arrival: 11:20:00

Time taken: 13 min (11:20 - 11:07)
Expected time: 30 min
Time difference: 17 min early! 😊

Penalty: -17 min × 3 (EarlyPenalty) = -51 points (negative = bonus)
```

**Total Penalties: 4 + 7 + (-51) = -40 points** ⭐

---

## 🟢 NEW METHOD (Single-Scan) - 2025 Style

### CSV Data Format:
```csv
# marshals/CP2.001.csv
Car Code,Time Captured
ART40/25/005,10:49:00    ← Only entry scanned, no exit scan!
```

### Checkpoint Calculations:

**CP1 (No Break):**
```
Expected arrival: 10:20:00 (10:00 + 20 min)
Actual arrival:   10:22:00
Departure:        10:22:00 (scanned)

Time difference: 2 min late
Penalty: 2 min × 2 (LatePenalty) = 4 points
```
*(Same as old method)*

**CP2 (15-min Break - SINGLE SCAN):**
```
Expected arrival: 10:47:00 (10:22 + 25 min)
Actual arrival:   10:49:00 (scanned)
Departure:        11:04:00 (CALCULATED: 10:49 + 15 min)
                          ↑
                  System assumes they took exactly 15 min break!

Arrival penalty:
  Time difference: 2 min late
  Penalty: 2 min × 2 (LatePenalty) = 4 points

Break penalty: 0 points (not monitored!)
                ↑
    Participant must manage their own break time

Total CP2 penalty: 4 points only
```

**CP3 (No Break):**
```
Expected arrival: 11:37:00 (10:47 + 15 + 30 min)
Previous departure: 11:04:00 (assumed from CP2)
                           ↑
               System thinks they left at 11:04
               But they actually left at 11:07 (3 min late)

Actual arrival: 11:20:00

Time taken: 16 min (11:20 - 11:04)
Expected time: 30 min
Time difference: 14 min early

Penalty: -14 min × 3 (EarlyPenalty) = -42 points
```

**Total Penalties: 4 + 4 + (-42) = -34 points** ⭐

---

## 📊 Comparison Summary

| Checkpoint | Old Method (Dual-Scan) | New Method (Single-Scan) | Difference |
|------------|------------------------|--------------------------|------------|
| **CP1** | 4 points | 4 points | Same ✓ |
| **CP2** | 7 points (4 arrival + 3 break) | 4 points (arrival only) | -3 points |
| **CP3** | -51 points | -42 points | +9 points |
| **TOTAL** | **-40 points** | **-34 points** | **+6 points** |

---

## 🎯 Key Insights

### What Happened at CP2?
- **Reality**: Participant took **18 minutes** break (3 min extra)
- **Old system**: Caught the 3-min overage, penalized 3 points at CP2
- **New system**: Assumed 15-min break, didn't penalize at CP2

### What Happened at CP3?
- **Reality**: Participant left CP2 at 11:07, took 13 min to reach CP3
- **Old system**: Knows real departure (11:07), calculates 13-min travel = very early (-17 min)
- **New system**: Assumes departure (11:04), calculates 16-min travel = still early but less (-14 min)

### The "Penalty Shift":
```
Old: 3 min break penalty at CP2 + super early at CP3 (-17 min)
New: 0 min break penalty at CP2 + less early at CP3 (-14 min)

The 3 extra minutes "moved" from break penalty to timing penalty!
```

---

## 💡 Practical Impact

### For Marshals:
- ✅ **Easier**: Only scan ONCE per car at break checkpoints
- ✅ **Faster**: Less time spent per car
- ✅ **Fewer errors**: No risk of missing exit scan

### For Participants:
- ⚠️ **Must manage break time themselves**
- ⚠️ Taking extra break appears as "late" at next checkpoint
- ⚠️ Leaving early appears as "early arrival" at next checkpoint
- ✅ **More transparent**: All timing in one place

### For Organizers:
- ✅ **Simpler process**
- ✅ **Backward compatible** with old data
- ⚠️ **Penalties slightly different** (but fair)

---

## 🔄 Backward Compatibility

The system automatically detects which method to use based on the scan data:

**Single-Scan Data** (new method):
```csv
ART40/25/005,10:49:00
```
- Triggers single-scan mode
- Departure calculated automatically
- No break penalty

**Dual-Scan Data** (old method):
```csv
ART40/25/005,10:49:00 | 11:07:00
```
- Triggers dual-scan mode
- Departure from actual scan
- Break penalty calculated

**No changes to CSV file format required!** The same format supports both methods.

---

## 📝 Recommendation

**Use single-scan method (2025 style)** for:
- Simpler marshal operations
- Faster checkpoint processing
- Putting responsibility on participants

**Use dual-scan method (2024 style)** for:
- Strict break time enforcement
- When you want to monitor actual break duration
- Backward compatibility with existing processes

The new method is simpler and puts responsibility on participants to manage their break time, while maintaining fair penalty calculations!
