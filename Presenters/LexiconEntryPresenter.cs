﻿using System;
using IPresenters;
using IServices;
using IViews;
using Models;
using LexServices;

namespace Presenters
{
    public class LexiconEntryPresenter<T, TService> : IPresenter
        where T: class, new()
        where TService: IService<T>
    {
        public EventHandler Create;
        public EventHandler Read;
        public EventHandler Update;
        public EventHandler Delete;
        public EventHandler Cancel;

        private ILexiconService<LexiconRaw> _service;

        private IViewLexiconEntryScreen _view;

        private bool _isAdding = false;

        public LexiconEntryPresenter(TService service)
        {
            _service = service as ILexiconService<LexiconRaw>;
        }

        public void Setup(IView view)
        {
            _view = view as IViewLexiconEntryScreen;

            Create += new EventHandler(Insert);
            _view.Insert += Insert;

            Delete += new EventHandler(Deactivate);
            _view.Insert += Insert;

            Update += new EventHandler(Save);
            _view.UpdateItem += Update;

            //_view.Datasource = _service.FindAll();

            _view.PageMoveCompleted += CompletedPageMove;
        }

        private void CompletedPageMove(object sender, EventArgs e)
        {
            if (_view.IsAddingNewRecord)
            {
                _isAdding = true;
                string s = "stop";
            }
            else
            {
                var currentData = _view.CurrentItem as LexiconRaw;
                var obtainedData = (currentData != null ? _service.FindItem(c => c.Id == currentData.Id) : null);
                if (obtainedData != null)
                {
                    PopulateView(obtainedData);
                }
            }
        }

        private void PopulateView(LexiconRaw item)
        {
            _view.Id = item.Id;
            _view.LanguageId = item.LanguageId.ToString();
            _view.Entry = item.Entry;
            _view.IPA = item.IPA;
            _view.Meaning = item.Meaning;
            _view.SecondaryMeanings = item.SecondaryMeanings;
            _view.Synonyms = item.Synonyms;

            _view.Dialect = item.Dialect;
            _view.Register = item.Register;

            _view.Gender = item.Gender;
            _view.NounIncorporatedForm = item.NounIncorporatedForm;
            _view.Pos = item.Pos;
            _view.PosSubtype = item.PosSubtype;
            _view.Domain = item.Domain;

            _view.Etymology = item.Etymology;
            _view.GrammaticalNotes = item.GrammaticalNotes;
            _view.AdditionalNotes = item.AdditionalNotes;
            _view.AlternateForms = item.AlternateForms;
            _view.EntryDate = item.EntryDate.ToString();
            _view.DeactivatedDate = item.DeactivatedDate.ToString();
        }

        private void Deactivate(object sender, EventArgs e)
        {
            //string sql = UpdateDeactivateRecord();
        }

        private LexiconRaw GetDataFromView()
        {
            LexiconRaw newItem = new LexiconRaw();
            if (!_isAdding)
            {
                newItem.Id = _view.Id;
            }

            int sInt = 0;
            newItem.LanguageId = (int.TryParse(_view.LanguageId, out sInt) ? sInt : 0);
            
            newItem.Entry = _view.Entry;
            newItem.IPA = _view.IPA;
            newItem.Meaning = _view.Meaning;
            newItem.SecondaryMeanings = _view.SecondaryMeanings;
            newItem.Synonyms = _view.Synonyms;

            newItem.Dialect = _view.Dialect;
            newItem.Register = _view.Register;

            newItem.Gender = _view.Gender;
            newItem.NounIncorporatedForm = _view.NounIncorporatedForm;
            newItem.Pos = _view.Pos;
            newItem.PosSubtype = _view.PosSubtype;
            newItem.Domain = _view.Domain;

            newItem.Etymology = _view.Etymology;
            newItem.GrammaticalNotes = _view.GrammaticalNotes;
            newItem.AdditionalNotes = _view.AdditionalNotes;
            newItem.AlternateForms = _view.AlternateForms;
            newItem.EntryDate = DateTime.Parse(!string.IsNullOrWhiteSpace(_view.EntryDate) ? _view.EntryDate : DateTime.Now.ToString());

            if (!string.IsNullOrWhiteSpace(_view.DeactivatedDate))
            {
                newItem.DeactivatedDate = DateTime.Parse(_view.DeactivatedDate);
            }

            return newItem;
        }

        private void Insert(object sender, EventArgs e)
        {
            if (_isAdding)
            {
                var newEntry = GetDataFromView();
                _service.Add(newEntry);
                _service.SaveAndCommit();
            }
            _isAdding = false;
        }
        
        private void Save(object sender, EventArgs e)
        {  
            if (_isAdding)
            {
                Insert(sender, e);
                return;
            }

            LexiconRaw newItem = GetDataFromView();
            var currentItem = _service.FindItem(c => c.Id == newItem.Id);

            currentItem.Entry = newItem.Entry;
            currentItem.IPA = newItem.IPA;
            currentItem.Meaning = newItem.Meaning;
            currentItem.SecondaryMeanings = newItem.SecondaryMeanings;
            currentItem.Synonyms = newItem.Synonyms;
            
            currentItem.Dialect = newItem.Dialect;
            currentItem.Register = newItem.Register;
            
            currentItem.Gender = newItem.Gender;
            currentItem.NounIncorporatedForm = newItem.NounIncorporatedForm;
            currentItem.Pos = newItem.Pos;
            currentItem.PosSubtype = newItem.PosSubtype;
            currentItem.Domain = newItem.Domain;
            
            currentItem.Etymology = newItem.Etymology;
            currentItem.GrammaticalNotes = newItem.GrammaticalNotes;
            currentItem.AdditionalNotes = newItem.AdditionalNotes;
            currentItem.AlternateForms = newItem.AlternateForms;
            currentItem.EntryDate = newItem.EntryDate;
            currentItem.DeactivatedDate = newItem.DeactivatedDate;

            _service.SaveAndCommit();
        }
    }
}
