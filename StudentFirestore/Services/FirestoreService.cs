using System;
using Google.Cloud.Firestore;
using StudentFirestore.Models;

namespace StudentFirestore.Services;

public class FirestoreService
{
    private FirestoreDb db;
    public string StatusMessage;

    public FirestoreService()
    {
        this.SetupFireStore(); 
    }
    private async Task SetupFireStore()
    {
        if (db == null)
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("dx212-firestore-firebase-adminsdk-uyyse-c42f3bf1c0.json");
            var reader = new StreamReader(stream);
            var contents = reader.ReadToEnd();
            db = new FirestoreDbBuilder
            {
                ProjectId = "dx212-firestore",

                JsonCredentials = contents
            }.Build();
        }
    }

    public async Task<List<StudentModel>> GetAllStudent()
    {
        try
        {
            await SetupFireStore();
            var data = await db.Collection("Students").GetSnapshotAsync();
            var Students = data.Documents.Select(doc =>
            {
                var student = new StudentModel();
                student.Id = doc.Id;
                student.Code = doc.GetValue<string>("Code");
                student.Name = doc.GetValue<string>("Name");
                return student;
            }).ToList();
            return Students;
        }
        catch (Exception ex)
        {

            StatusMessage = $"Error: {ex.Message}";
        }
        return null;
    }

    public async Task InsertSample(StudentModel student)
    {
        try
        {
            await SetupFireStore();
            var studentData = new Dictionary<string, object>
            {
                { "Code", student.Code },
                { "Name", student.Name }
                // Add more fields as needed
            };

            await db.Collection("Students").AddAsync(studentData);
        }
        catch (Exception ex)
        {

            StatusMessage = $"Error: {ex.Message}";
        }
    }

    public async Task UpdateSample(StudentModel student)
    {
        try
        {
            await SetupFireStore();

            // Manually create a dictionary for the updated data
            var studentData = new Dictionary<string, object>
            {
                { "Code", student.Code },
                { "Name", student.Name }
                // Add more fields as needed
            };

            // Reference the document by its Id and update it
            var docRef = db.Collection("Students").Document(student.Id);
            await docRef.SetAsync(studentData, SetOptions.Overwrite);

            StatusMessage = "Student successfully updated!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    public async Task DeleteSample(string id)
    {
        try
        {
            await SetupFireStore();

            // Reference the document by its Id and delete it
            var docRef = db.Collection("Students").Document(id);
            await docRef.DeleteAsync();

            StatusMessage = "Student successfully deleted!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }




}

