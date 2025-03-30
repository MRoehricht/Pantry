//
//  EditRecipeView.swift
//  Pantry
//
//  Created by Matthias Röhricht on 30.03.25.
//

import SwiftUI
import SwiftData

struct EditRecipeView: View {
    @Binding var recipe: Recipe
    @State private var ingredientName: String = ""
    @State private var ingredientQuantity: String = ""
    @State private var selectedImage: UIImage?
    @State private var showImagePicker: Bool = false
    @Environment(\.modelContext) private var modelContext

    var body: some View {
        Form {
            Section(header: Text("Rezeptname")) {
                TextField("Name", text: $recipe.name)
            }
            Section(header: Text("Zutaten")) {
                if let ingredients = recipe.ingredients {
                    ForEach(ingredients.indices, id: \.self) { index in
                        HStack {
                            TextField("Zutat", text: Binding(
                                get: { ingredients[index].name },
                                set: { ingredients[index].name = $0 }
                            ))
                            TextField("Menge", text: Binding(
                                get: { ingredients[index].quantity },
                                set: { ingredients[index].quantity = $0 }
                            ))
                        }
                    }
                }
                HStack {
                    TextField("Zutat", text: $ingredientName)
                    TextField("Menge", text: $ingredientQuantity)
                    Button(action: addIngredient) {
                        Text("Hinzufügen")
                    }
                }
            }
            Section(header: Text("Bild")) {
                HStack {
                    Spacer()
                    if let imageData = recipe.imageData, let uiImage = UIImage(data: imageData) {
                        Image(uiImage: uiImage)
                            .resizable()
                            .scaledToFit()
                            .frame(height: 200)
                    }
                    Spacer()
                }
                Button(action: {
                    showImagePicker = true
                }) {
                    Text("Bild auswählen")
                }
            }
        }
        .navigationTitle("Rezept bearbeiten")
        .sheet(isPresented: $showImagePicker) {
            ImagePicker(image: $selectedImage)
        }
        .onChange(of: selectedImage) { newImage in
            if let newImage = newImage {
                recipe.imageData = newImage.jpegData(compressionQuality: 0.8)
            }
        }
        .onDisappear {
            saveRecipe()
        }
    }

    private func addIngredient() {
        let newIngredient = Ingredient(name: ingredientName, quantity: ingredientQuantity)
        recipe.ingredients?.append(newIngredient)
        ingredientName = ""
        ingredientQuantity = ""
    }

    private func saveRecipe() {
        recipe.ingredients?.removeAll { $0.name.isEmpty && $0.quantity.isEmpty }
        do {
            try modelContext.save()
        } catch {
            print("Failed to save recipe: \(error)")
        }
    }
}

#Preview {
    EditRecipeView(recipe: .constant(Recipe(name: "Test", ingredients: [])))
        .modelContainer(for: [Recipe.self, Ingredient.self])
}
