// RentLogic.cs
using System.Collections.Generic;
using System.Linq;
using Core;
using Scripts.EcoSystem.Calendar;
using Scripts.GlobalStateMachine;
using Scripts.Progress;
using Scripts.Wallet;
using UnityEngine;

namespace _root.RealEstate
{
    public class RentLogic : ICleanUp
    {
        private const string CurrentFlatIndexKey = "CurrentFlatIndex";

        private readonly LocalEvents _localEvents;
        private readonly CalendarLogic _calendarLogic;
        private readonly ProgressDataAdapter _progress;

        private List<Flat> _flats;
        private IFlat _currentFlat;

        public RentLogic(LocalEvents localEvents, CalendarLogic calendarLogic, ProgressDataAdapter progress)
        {
            _localEvents   = localEvents;
            _calendarLogic = calendarLogic;
            _progress      = progress;

            _localEvents.OnNewDay += CheckDayToPay;
            TryLoadApartment();
        }

        public void TryLoadApartment() 
        {
            _flats = FlatLoader.LoadAll();
            if (_flats == null || _flats.Count == 0)
            {
                Debug.LogWarning("[Rent] No flats found");
                return;
            }

            int index = 0;
            if (_progress.GetProgressData().Metadata.TryGetValue(CurrentFlatIndexKey, out var meta))
            {
                index = Mathf.Clamp((int)meta.Value, 0, _flats.Count - 1);
            }
            else
            {
                TrySaveCurrentFlatIndex(0);
            }

            SetCurrentFlatByIndex(index);
        }

        public void SetCurrentFlatByIndex(int index)
        {
            if (_flats == null || _flats.Count == 0) return;
            index = Mathf.Clamp(index, 0, _flats.Count - 1);
            _currentFlat = _flats[index];

            var flatTransaction = new Transaction
            {
                Amount = -_currentFlat.MonthPayment,
                DaysForTransaction = new[] { _currentFlat.DayToPay },
                Description = _currentFlat.Description,
                Name = "Apartment",
            };
            
//            Debug.Log("transaction created in rent logic");
            
            _localEvents.TriggerNewTransaction(flatTransaction);
            TrySaveCurrentFlatIndex(index);
        }

        private void TrySaveCurrentFlatIndex(int index)
        {
            if (_progress.GetProgressData().Metadata.TryGetValue(CurrentFlatIndexKey, out var meta))
            {
                meta.Value = index; 
            }
            else
            {
                Debug.LogWarning($"[Rent] Meta key '{CurrentFlatIndexKey}' not found. Add it to meta_config.json");
            }
        }

        private void CheckDayToPay()
        {
            if (_currentFlat == null) return;

            var today = _calendarLogic.GetCurrentDate().Day;
            if (today == _currentFlat.DayToPay)
            {
                _localEvents.TriggerRentPayDay(_currentFlat.MonthPayment);
                Debug.Log($"[Rent] Pay day → request ${_currentFlat.MonthPayment}");
            }
        }

        public void CleanUp()
        {
            _localEvents.OnNewDay -= CheckDayToPay;
        }
    }
}
